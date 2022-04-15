using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreationModelPlugin
{
    public class SelObjModel
    {
        #region Выбор уровня
        public static void SelectLevels(Document doc, out Level level1, out Level level2)
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();
            level1 = listLevel
                .Where(x => x.Name.Equals("Уровень 1"))
                .FirstOrDefault();
            level2 = listLevel
                .Where(x => x.Name.Equals("Уровень 2"))
                .FirstOrDefault();
        }
        #endregion

        #region Выбор двери
        public static FamilySymbol SelectDoor(Document doc)
        { 
            FamilySymbol doorType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Doors)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals("0915 x 2134 мм"))
                .Where(x => x.FamilyName.Equals("Одиночные-Щитовые"))
                .FirstOrDefault();
            return doorType;
        }
        #endregion
        
        #region Выбор окон
        public static FamilySymbol SelectWin(Document doc)
        {
            FamilySymbol winType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals("0915 x 1830 мм"))
                .Where(x => x.FamilyName.Equals("Фиксированные"))
                .FirstOrDefault();
            return winType;
        }
        #endregion

        #region Выбор крыши
        public static RoofType SelectRoof(Document doc)
        {
            RoofType roofType = new FilteredElementCollector(doc)
                 .OfClass(typeof(RoofType))
                 .OfType<RoofType>()
                 .Where(x => x.Name.Equals("Типовой - 400мм"))
                 .Where(x => x.FamilyName.Equals("Базовая крыша"))
                 .FirstOrDefault();
            return roofType;
        }
        #endregion
    }
}
