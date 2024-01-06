using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using Point = Autodesk.Revit.DB.Point;

namespace PluginBuildingConstructionReinforcement.Model.Create
{
    [Transaction(TransactionMode.Manual)]
    public sealed class ElementFactory
    {
        private readonly Document _document;
        private double _offsetRod;
        private double _lenghtBent;
        private double _rodDisplacement;

        public ElementFactory(Document document) 
        {
            _document = document;
        }

        public void SetData(double offsetRod, double lenghtBent, double rodDisplacement)
        {
            _offsetRod = offsetRod;
            _lenghtBent = lenghtBent;
            _rodDisplacement = rodDisplacement;
        }

        public void Building(IList<Reference> references)
        {
            foreach (var element in references)
            {
                BuildingAnotherShape(element);
            }
        }
      
        private void BuildingAnotherShape(Reference reference)
        {
            Element element = _document.GetElement(reference);

            Location pointElement = element.Location;
            LocationPoint pointer = pointElement as LocationPoint;

            XYZ origin = new XYZ(pointer.Point.X, pointer.Point.Y, pointer.Point.Z);

            XYZ offesPosition1 = new XYZ
                (
                    origin.X,
                    origin.Y,
                    origin.Z + _offsetRod
                );

            XYZ offesPosition2 = new XYZ
                (
                    offesPosition1.X,
                    offesPosition1.Y + _rodDisplacement,
                    offesPosition1.Z + _lenghtBent
                );

            XYZ offesPosition3 = new XYZ
                (
                    offesPosition2.X,
                    offesPosition2.Y,
                    offesPosition2.Z + _rodDisplacement
            );



            List<XYZ> pointers = new List<XYZ>
            {
                origin,
                offesPosition1,
                offesPosition2,
                offesPosition3
            };

            List<Line> Lines = new List<Line>();

            for (int i = 1; i < pointers.Count; i++)
            {
                Lines.Add(Line.CreateBound(pointers[i - 1], pointers[i]));
            }

            try
            {
                Transaction transaction = new Transaction(_document, "Create shape");
                transaction.Start();

                for (int i = 0; i < Lines.Count; i++)
                {
                    CreateDirectShape(new List<GeometryObject>() { Lines[i] });
                }


                transaction.Commit();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.ToString());
            }
            finally
            {
                MessageBox.Show($"Генерация прошла успешно");
            }



        }

        private DirectShape CreateDirectShape(List<GeometryObject> geometryObject, BuiltInCategory category = BuiltInCategory.OST_GenericModel)
        {
            var directShape = DirectShape.CreateElement(_document, new ElementId(category));

            directShape.SetShape(geometryObject);

            return directShape;
        }


    }
}
