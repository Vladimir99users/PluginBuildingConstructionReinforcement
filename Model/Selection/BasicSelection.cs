using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;

namespace PluginBuildingConstructionReinforcement.Model.Selection
{
    public abstract class BasicSelection : ISelectionFilter
    {
        protected readonly Func<Element, bool> _validateElement;
        protected BasicSelection(Func<Element, bool> validatelement) 
        {
            _validateElement = validatelement;
        }

        public abstract bool AllowElement(Element elem);
        public abstract bool AllowReference(Reference reference, XYZ position);
    }
}
