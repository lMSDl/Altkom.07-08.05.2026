using DAL;
using GeoCoordinatePortable;
using Models;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    internal class Spatial
    {
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DAL.Context> config)
        {
            Transactions.Run(config, false);
            config.LogTo(Console.WriteLine);

            using var context = new Context(config.Options);
            var order = context.Set<Order>().FirstOrDefault();

            var point = new Point(51, 19) { SRID = 4326 };

            var isWithinDistance =  order.DeliveryPoint.IsWithinDistance(point, 1);

            //rezultat w "jednostce" - zawsze przestrzeń euklidesowa
            //w naszym przypadku jednostka to stopnie, co oznacza, że w SRID około  111000m na stopień
            var distance = order.DeliveryPoint.Distance(point);

            // distance * 111000 ~ sqlDistance

            //resultat w metrach - SQL Server uwzględnia SRID
            distance = context.Set<Order>().Where(x => x.Id == order.Id)
                .Select(x => x.DeliveryPoint.Distance(point))
                .FirstOrDefault();

            //rezultat w metrach (GeoCoordinate.NET)
            var geo1 = new GeoCoordinate(order.DeliveryPoint.Y, order.DeliveryPoint.X);
            var geo2 = new GeoCoordinate(point.Y, point.X);
            var geoCordinateDistance = geo1.GetDistanceTo(geo2);


            var polygon = new Polygon(new LinearRing(new Coordinate[] { new Coordinate(51, 18),
                                                                            new Coordinate(52, 19),
                                                                            new Coordinate(51, 20),
                                                                            new Coordinate(50, 19),
                                                                            new Coordinate(51, 18)}))
            { SRID = 4326 };

            var intersects = order.DeliveryPoint.Intersects(polygon);
            intersects = point.Intersects(polygon);

            var orders = context.Set<Order>()
    .Where(o => o.DeliveryPoint.Intersects(polygon))
    .ToList();
        }
    }
}
