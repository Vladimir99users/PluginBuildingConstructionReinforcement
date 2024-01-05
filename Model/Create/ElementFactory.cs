using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using Point = Autodesk.Revit.DB.Point;

namespace PluginBuildingConstructionReinforcement.Model.Create
{
    [Transaction(TransactionMode.Manual)]
    public sealed class ElementFactory
    {
        private readonly Document _document;

        public ElementFactory(Document document) 
        {
            _document = document;
        }

        public void GetLocationInformation(double x, double y, double z)
        {
            MessageBox.Show("Factory is Worked");

            

        }
    }
}
