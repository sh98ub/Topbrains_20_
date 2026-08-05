using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Q47_MetroSmartCardSystem
{
    // Provided Classes (Do Not Modify)
    public class TravelSummary
    {
        public long lastEntryStation;
        public long lastExitStation;
        public long lastEntryTime;
        public long lastExitTime;
        public double totalFarePaid;
        public int totalTrips;
        public double averageFarePerTrip;
    }

    public class Commuter
    {
        public int cardNumber;
        public string commuterName = string.Empty;
        public string commuterType = string.Empty; // "SENIOR", "ADULT", "STUDENT", "CHILD"
        public TravelSummary travelSummary = new TravelSummary();
    }

    public class Station
    {
        public int stationId;
        public string stationName = string.Empty;
        public int zone; // 1, 2, or 3
        public double latitude;
        public double longitude;
    }

    public interface MetroOperations
    {
        void issueCard(int cardNumber, string commuterName, string commuterType);
        bool tapIn(int cardNumber, int stationId, long epochTime);
        bool tapOut(int cardNumber, int stationId, long epochTime);
        Commuter getCommuterInfo(int cardNumber);
        List<double> fareHistory(int cardNumber);
        Dictionary<string, double> getZoneWiseRevenue(long startTime, long endTime);
        List<string> getFrequentRoute(int cardNumber);
        double getDailyPassSavings(int cardNumber, long date);
    }

    public class MetroCardManager : MetroOperations
    {
        private readonly Dictionary<int, Station> _stations = new Dictionary<int, Station>();
        private readonly double _baseFare;
        private readonly double _perKmRate;
        private readonly double _maxDailyCap;

        private readonly Dictionary<int, Commuter> _commuters = new Dictionary<int, Commuter>();
        private readonly Dictionary<int, ActiveJourney> _activeJourneys = new Dictionary<int, ActiveJourney>();
        private readonly Dictionary<int, List<double>> _commuterFares = new Dictionary<int, List<double>>();
        private readonly Dictionary<int, Dictionary<string, int>> _routeCounts = new Dictionary<int, Dictionary<string, int>>();
        private readonly Dictionary<int, Dictionary<long, double>> _dailyFares = new Dictionary<int, Dictionary<long, double>>();
        private readonly List<CompletedJourney> _completedJourneys = new List<CompletedJourney>();

        private class ActiveJourney
        {
            public int EntryStationId;
            public long EntryTime;
        }

        private class CompletedJourney
        {
            public int CardNumber;
            public int EntryStationId;
            public int ExitStationId;
            public int EntryZone;
            public int ExitZone;
            public long EntryTime;
            public long ExitTime;
            public double FarePaid;
        }

        public MetroCardManager(List<Station> stations, double baseFare, double perKmRate, double maxDailyCap)
        {
            _baseFare = baseFare;
            _perKmRate = perKmRate;
            _maxDailyCap = maxDailyCap;

            if (stations != null)
            {
                foreach (var s in stations)
                {
                    _stations[s.stationId] = s;
                }
            }
        }

        public void issueCard(int cardNumber, string commuterName, string commuterType)
        {
            var summary = new TravelSummary
            {
                lastEntryStation = 0,
                lastExitStation = 0,
                lastEntryTime = 0,
                lastExitTime = 0,
                totalFarePaid = 0.0,
                totalTrips = 0,
                averageFarePerTrip = 0.0
            };

            var commuter = new Commuter
            {
                cardNumber = cardNumber,
                commuterName = commuterName,
                commuterType = commuterType,
                travelSummary = summary
            };

            _commuters[cardNumber] = commuter;
            _commuterFares[cardNumber] = new List<double>();
            _routeCounts[cardNumber] = new Dictionary<string, int>();
        }

        public bool tapIn(int cardNumber, int stationId, long epochTime)
        {
            if (!_commuters.ContainsKey(cardNumber)) return false;
            if (_activeJourneys.ContainsKey(cardNumber)) return false;
            if (!_stations.ContainsKey(stationId)) return false;

            _activeJourneys[cardNumber] = new ActiveJourney
            {
                EntryStationId = stationId,
                EntryTime = epochTime
            };

            var commuter = _commuters[cardNumber];
            commuter.travelSummary.lastEntryStation = stationId;
            commuter.travelSummary.lastEntryTime = epochTime;

            return true;
        }

        public bool tapOut(int cardNumber, int stationId, long epochTime)
        {
            if (!_commuters.ContainsKey(cardNumber)) return false;
            if (!_activeJourneys.TryGetValue(cardNumber, out var active)) return false;
            if (!_stations.ContainsKey(stationId)) return false;
            if (epochTime < active.EntryTime) return false;
            if (active.EntryStationId == stationId) return false;

            var entryStation = _stations[active.EntryStationId];
            var exitStation = _stations[stationId];

            double distance = calculateDistance(entryStation, exitStation);
            double durationMinutes = (epochTime - active.EntryTime) / (1000.0 * 60.0);

            double rawFare;
            if (durationMinutes > 120.0)
            {
                rawFare = _baseFare * 3.0;
            }
            else
            {
                rawFare = _baseFare + (distance * _perKmRate);
            }

            double discountRate = GetDiscountRate(_commuters[cardNumber].commuterType);
            double discountedFare = rawFare * (1.0 - discountRate);

            long dateKey = GetDateAsLong(active.EntryTime);

            if (!_dailyFares.ContainsKey(cardNumber))
            {
                _dailyFares[cardNumber] = new Dictionary<long, double>();
            }
            if (!_dailyFares[cardNumber].ContainsKey(dateKey))
            {
                _dailyFares[cardNumber][dateKey] = 0.0;
            }

            double currentDayTotal = _dailyFares[cardNumber][dateKey];
            double finalFare;

            if (currentDayTotal >= _maxDailyCap)
            {
                finalFare = 0.0;
            }
            else if (currentDayTotal + discountedFare > _maxDailyCap)
            {
                finalFare = _maxDailyCap - currentDayTotal;
            }
            else
            {
                finalFare = discountedFare;
            }

            _dailyFares[cardNumber][dateKey] += finalFare;

            var commuter = _commuters[cardNumber];
            commuter.travelSummary.lastExitStation = stationId;
            commuter.travelSummary.lastExitTime = epochTime;
            commuter.travelSummary.totalFarePaid += finalFare;
            commuter.travelSummary.totalTrips += 1;
            commuter.travelSummary.averageFarePerTrip = commuter.travelSummary.totalFarePaid / commuter.travelSummary.totalTrips;

            _commuterFares[cardNumber].Add(finalFare);

            string routeName = $"{entryStation.stationName} to {exitStation.stationName}";
            if (!_routeCounts[cardNumber].ContainsKey(routeName))
            {
                _routeCounts[cardNumber][routeName] = 0;
            }
            _routeCounts[cardNumber][routeName]++;

            _completedJourneys.Add(new CompletedJourney
            {
                CardNumber = cardNumber,
                EntryStationId = active.EntryStationId,
                ExitStationId = stationId,
                EntryZone = entryStation.zone,
                ExitZone = exitStation.zone,
                EntryTime = active.EntryTime,
                ExitTime = epochTime,
                FarePaid = finalFare
            });

            _activeJourneys.Remove(cardNumber);
            return true;
        }

        public Commuter getCommuterInfo(int cardNumber)
        {
            if (_commuters.TryGetValue(cardNumber, out var c))
            {
                return c;
            }
            return null!;
        }

        public List<double> fareHistory(int cardNumber)
        {
            if (!_commuterFares.ContainsKey(cardNumber) || _commuterFares[cardNumber].Count == 0)
            {
                return new List<double>();
            }

            var list = _commuterFares[cardNumber];
            int takeCount = Math.Min(5, list.Count);
            var last5 = list.Skip(list.Count - takeCount).ToList();
            last5.Sort((a, b) => b.CompareTo(a));
            return last5;
        }

        public Dictionary<string, double> getZoneWiseRevenue(long startTime, long endTime)
        {
            var revenueMap = new Dictionary<string, double>();

            foreach (var j in _completedJourneys)
            {
                if (j.ExitTime >= startTime && j.ExitTime <= endTime)
                {
                    string key = $"Zone{j.EntryZone}-Zone{j.ExitZone}";
                    if (!revenueMap.ContainsKey(key))
                    {
                        revenueMap[key] = 0.0;
                    }
                    revenueMap[key] += j.FarePaid;
                }
            }

            var sortedList = revenueMap
                .Where(kv => kv.Value > 0)
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .ToList();

            var result = new Dictionary<string, double>();
            foreach (var item in sortedList)
            {
                result[item.Key] = item.Value;
            }

            return result;
        }

        public List<string> getFrequentRoute(int cardNumber)
        {
            if (!_routeCounts.ContainsKey(cardNumber) || _routeCounts[cardNumber].Count == 0)
            {
                return new List<string>();
            }

            var map = _routeCounts[cardNumber];
            var sortedRoutes = map
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .Take(3)
                .ToList();

            return sortedRoutes;
        }

        public double getDailyPassSavings(int cardNumber, long date)
        {
            double actualSpent = 0.0;
            if (_dailyFares.ContainsKey(cardNumber) && _dailyFares[cardNumber].TryGetValue(date, out double spent))
            {
                actualSpent = spent;
            }

            if (actualSpent <= 0.0)
            {
                return 0.0;
            }

            double passCost = _maxDailyCap * 0.8;
            double savings = actualSpent - passCost;

            return savings > 0 ? savings : 0.0;
        }

        private double calculateDistance(Station s1, Station s2)
        {
            double lat1 = Math.PI * s1.latitude / 180.0;
            double lon1 = Math.PI * s1.longitude / 180.0;
            double lat2 = Math.PI * s2.latitude / 180.0;
            double lon2 = Math.PI * s2.longitude / 180.0;

            double dlat = lat2 - lat1;
            double dlon = lon2 - lon1;

            double a = Math.Pow(Math.Sin(dlat / 2.0), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dlon / 2.0), 2);

            double c = 2.0 * Math.Asin(Math.Sqrt(a));
            double r = 6371.0;

            return r * c;
        }

        private double GetDiscountRate(string commuterType)
        {
            if (string.IsNullOrEmpty(commuterType)) return 0.0;
            switch (commuterType.ToUpper())
            {
                case "SENIOR": return 0.50;
                case "STUDENT": return 0.25;
                case "CHILD": return 0.75;
                case "ADULT": return 0.0;
                default: return 0.0;
            }
        }

        private long GetDateAsLong(long epochMillis)
        {
            DateTimeOffset dto = DateTimeOffset.FromUnixTimeMilliseconds(epochMillis);
            return (long)dto.UtcDateTime.Year * 10000 + dto.UtcDateTime.Month * 100 + dto.UtcDateTime.Day;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string firstLine = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(firstLine)) return;

            var firstTokens = ParseTokens(firstLine);
            if (firstTokens.Count < 4) return;

            int numberOfRequests = int.Parse(firstTokens[0]);
            double baseFare = double.Parse(firstTokens[1], CultureInfo.InvariantCulture);
            double perKmRate = double.Parse(firstTokens[2], CultureInfo.InvariantCulture);
            double maxDailyCap = double.Parse(firstTokens[3], CultureInfo.InvariantCulture);

            string stationsLine = Console.ReadLine()!;
            int numberOfStations = int.Parse(stationsLine.Trim());

            var stations = new List<Station>();
            for (int i = 0; i < numberOfStations; i++)
            {
                string sLine = Console.ReadLine()!;
                var sTokens = ParseTokens(sLine);
                stations.Add(new Station
                {
                    stationId = int.Parse(sTokens[0]),
                    stationName = sTokens[1],
                    zone = int.Parse(sTokens[2]),
                    latitude = double.Parse(sTokens[3], CultureInfo.InvariantCulture),
                    longitude = double.Parse(sTokens[4], CultureInfo.InvariantCulture)
                });
            }

            var manager = new MetroCardManager(stations, baseFare, perKmRate, maxDailyCap);

            for (int i = 0; i < numberOfRequests; i++)
            {
                string cmdLine = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(cmdLine)) continue;

                var cmdTokens = ParseTokens(cmdLine);
                if (cmdTokens.Count == 0) continue;

                string cmd = cmdTokens[0];

                switch (cmd)
                {
                    case "issueCard":
                        manager.issueCard(int.Parse(cmdTokens[1]), cmdTokens[2], cmdTokens[3]);
                        break;

                    case "tapIn":
                        bool inRes = manager.tapIn(int.Parse(cmdTokens[1]), int.Parse(cmdTokens[2]), long.Parse(cmdTokens[3]));
                        Console.WriteLine(inRes ? "true" : "false");
                        break;

                    case "tapOut":
                        bool outRes = manager.tapOut(int.Parse(cmdTokens[1]), int.Parse(cmdTokens[2]), long.Parse(cmdTokens[3]));
                        Console.WriteLine(outRes ? "true" : "false");
                        break;

                    case "commuterInfo":
                        var c = manager.getCommuterInfo(int.Parse(cmdTokens[1]));
                        if (c != null)
                        {
                            Console.WriteLine($"{c.cardNumber} {c.commuterName} {c.commuterType} {c.travelSummary.lastEntryStation} {c.travelSummary.lastExitStation} {c.travelSummary.lastEntryTime} {c.travelSummary.lastExitTime} {FormatDouble(c.travelSummary.totalFarePaid)} {c.travelSummary.totalTrips} {FormatDouble(c.travelSummary.averageFarePerTrip)}");
                        }
                        break;

                    case "fareHistory":
                        var history = manager.fareHistory(int.Parse(cmdTokens[1]));
                        foreach (var f in history)
                        {
                            Console.WriteLine(FormatDouble(f));
                        }
                        break;

                    case "zoneRevenue":
                        var rev = manager.getZoneWiseRevenue(long.Parse(cmdTokens[1]), long.Parse(cmdTokens[2]));
                        foreach (var kvp in rev)
                        {
                            Console.WriteLine($"{kvp.Key}:{FormatDouble(kvp.Value)}");
                        }
                        break;

                    case "frequentRoute":
                        var routes = manager.getFrequentRoute(int.Parse(cmdTokens[1]));
                        foreach (var r in routes)
                        {
                            Console.WriteLine(r);
                        }
                        break;

                    case "dailySavings":
                        double savings = manager.getDailyPassSavings(int.Parse(cmdTokens[1]), long.Parse(cmdTokens[2]));
                        Console.WriteLine(FormatDouble(savings));
                        break;
                }
            }
        }

        private static string FormatDouble(double value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private static List<string> ParseTokens(string line)
        {
            var tokens = new List<string>();
            if (string.IsNullOrWhiteSpace(line)) return tokens;

            bool inQuotes = false;
            var sb = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (sb.Length > 0)
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
            {
                tokens.Add(sb.ToString());
            }

            return tokens;
        }
    }
}
