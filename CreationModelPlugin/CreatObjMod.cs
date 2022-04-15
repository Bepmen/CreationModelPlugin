using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreationModelPlugin
{
    public class CreatObjMod
    {
        #region Построение объекта
        
        public static void CreateObj(Document doc, Level level1, Level level2)
        {
            double width = UnitUtils.ConvertToInternalUnits(10000, UnitTypeId.Millimeters);
            double depth = UnitUtils.ConvertToInternalUnits(5000, UnitTypeId.Millimeters);
            double dx = width / 2;
            double dy = depth / 2;

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));

            List<Wall> walls = new List<Wall>();

            Transaction transaction = new Transaction(doc, "Построение стен");
            transaction.Start();
            //Построение стен
            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);
                walls.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
            }
            // Построение двери
            CreateDoor(doc, level1, walls[0]);
            // Построение окон
            CreateWindow(doc, level1, walls[1]);
            CreateWindow(doc, level1, walls[2]);
            CreateWindow(doc, level1, walls[3]);
            // Построение крыши
            CreateRoof(doc, level2, walls);
            transaction.Commit();
        }
        #endregion

        #region Создание двери
        public static void CreateDoor(Document doc, Level level1, Wall wall)
        { 
            var doorType = SelObjModel.SelectDoor(doc);
            LocationCurve hostCurve = wall.Location as LocationCurve;
            XYZ point1 = hostCurve.Curve.GetEndPoint(0);
            XYZ point2 = hostCurve.Curve.GetEndPoint(1);
            XYZ point = (point1 + point2) / 2;
            if (!doorType.IsActive)
                doorType.Activate();
            doc.Create.NewFamilyInstance(point, doorType, wall, level1, StructuralType.NonStructural);
        }
        #endregion

        #region Создание окон
        private static void CreateWindow(Document doc, Level level1, Wall wall)
        {
            var winType = SelObjModel.SelectWin(doc);
            LocationCurve hostCurve = wall.Location as LocationCurve;
            XYZ point1 = hostCurve.Curve.GetEndPoint(0);
            XYZ point2 = hostCurve.Curve.GetEndPoint(1);
            XYZ point = (point1 + point2) / 2;
            if (!winType.IsActive)
                winType.Activate();
            var window = doc.Create.NewFamilyInstance(point, winType, wall, level1, StructuralType.NonStructural);
            Parameter sillHeight = window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM);
            double sh = UnitUtils.ConvertToInternalUnits(900, UnitTypeId.Millimeters);
            sillHeight.Set(sh);
        }
        #endregion

        #region Создание крыши
        private static void CreateRoof(Document doc, Level level2, List<Wall> walls)
        {
            var roofType = SelObjModel.SelectRoof(doc);
            double wallWidth = walls[0].Width;
            double dw = wallWidth / 2;
            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dw, -dw, 0));
            points.Add(new XYZ(dw, -dw, 0));
            points.Add(new XYZ(dw, dw, 0));
            points.Add(new XYZ(-dw, dw, 0));
            points.Add(new XYZ(-dw, -dw, 0));

            Application application = doc.Application;
            CurveArray footprint = application.Create.NewCurveArray();
            for (int i = 0; i < 4; i++)
            {
                LocationCurve curve = walls[i].Location as LocationCurve;
                XYZ p1 = curve.Curve.GetEndPoint(0);
                XYZ p2 = curve.Curve.GetEndPoint(1);
                Line line = Line.CreateBound(p1 + points[i], p2 + points[i + 1]);
                footprint.Append(line);
            }
            /*ModelCurveArray footPrintToModelCurveMapping = new ModelCurveArray();
            FootPrintRoof footprintRoof = doc.Create.NewFootPrintRoof(footprint, level2, roofType, out footPrintToModelCurveMapping);
            foreach (ModelCurve m in footPrintToModelCurveMapping)
            {
                footprintRoof.set_DefinesSlope(m, true);
                footprintRoof.set_SlopeAngle(m, 0.5);
            }
            */
            CurveArray curveArray = new CurveArray();
            curveArray.Append(Line.CreateBound(new XYZ(-16.73, -8.53, 13.12), new XYZ(-16.73, 0, 19.69)));
            curveArray.Append(Line.CreateBound(new XYZ(-16.73, 0, 19.69), new XYZ(-16.73, 8.53, 13.12)));
            ReferencePlane plane = doc.Create.NewReferencePlane(new XYZ(0, 0, 0), new XYZ(0, 0, 20), new XYZ(0, 20, 0), doc.ActiveView);
            doc.Create.NewExtrusionRoof(curveArray, plane, level2, roofType, -16.73, 16.73);
        }
        #endregion
    }
}
