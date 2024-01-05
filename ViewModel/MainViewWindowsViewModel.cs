using System.Windows;
using Autodesk.Revit.DB;
using PluginBuildingConstructionReinforcement.Model.Create;
using PluginBuildingConstructionReinforcement.Model.Selection;

namespace PluginBuildingConstructionReinforcement
{
    public class MainViewWindowsViewModel : BasicViewModel
    {
        private readonly Document _document;
        private BuilderReinforcement _selection;


        public MainViewWindowsViewModel(Document document, BuilderReinforcement selection)
        {
            _document = document;
            _selection = selection;
        }


        private bool _isCurve;
        public bool IsCurve
        {
            get => _isCurve;
            set
            {
                _isCurve = value;
                OnPropertyChanged();
            }
        }

        private string _axisX;
        public string AxisX
        {
            get => _axisX;
            set
            {
                _axisX = value;
                OnPropertyChanged();
            }
        }

        private string _axisY;
        public string AxisY
        {
            get => _axisY;
            set
            {
                _axisY =value;
                OnPropertyChanged();
            }
        }

        private string _axisZ;
        public string AxisZ
        {
            get => _axisZ;
            set
            {
                _axisZ = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand BuildingCommand => new RelayCommand(action => BuildingLittleBox(), canExecute => { return true; });


        private void BuildingLittleBox()
        {
           // _selection.SetShapeFromLocation();
        }
    }
}
