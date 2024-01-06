using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using Autodesk.Revit.DB;
using PluginBuildingConstructionReinforcement.Model.Create;
using PluginBuildingConstructionReinforcement.Model.Selection;

namespace PluginBuildingConstructionReinforcement
{
    public class MainViewWindowsViewModel : BasicViewModel
    {
        private readonly Document _document;
        private readonly ElementFactory _factory;
        private readonly IList<Reference> _references;


        public MainViewWindowsViewModel(Document document, IList<Reference> references, ElementFactory factory)
        {
            _document = document;
            _factory = factory;
            _references = references;
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

        //отступ стержня от нижнего уровня плиты
        private float _offsetRodFromLowerLevelPlate;
        public float OffsetRodFromLowerLevelPlate
        {
            get => _offsetRodFromLowerLevelPlate;
            set
            {
                _offsetRodFromLowerLevelPlate = value;
                OnPropertyChanged();
            }
        }

        //длинна гнутого участка
        private float _lengthBentSection;
        public float LengthBentSection
        {
            get => _lengthBentSection;
            set
            {
                _lengthBentSection =value;
                OnPropertyChanged();
            }
        }

        //Смещение стержня
        private float _rodDisplacement;
        public float RodDisplacement
        {
            get => _rodDisplacement;
            set
            {
                _rodDisplacement = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand BuildingCommand => new RelayCommand(action => BuildingLittleBox(), canExecute => { return true; });


        private void BuildingLittleBox()
        {
            _factory.SetData(OffsetRodFromLowerLevelPlate, LengthBentSection, RodDisplacement);
            _factory.Building(_document,  _references);
        }
    }
}
