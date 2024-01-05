using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBuildingConstructionReinforcement.Model.Selection
{
    public sealed class ColumnSelection : BasicSelection
    {
        private readonly Func<Reference, bool> _validateReference;
        public ColumnSelection(Func<Element, bool> validatelement, Func<Reference, bool> validateReference) : base(validatelement)
        {
            _validateReference = validateReference;
        }

        public override bool AllowElement(Element elem)
        {
            return _validateElement(elem);
        }

        public override bool AllowReference(Reference reference, XYZ position)
        {
            return _validateReference?.Invoke(reference) ?? true;
        }
    }
}
