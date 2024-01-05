using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.SqlServer.Server;
using PluginBuildingConstructionReinforcement.Model.Selection;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PluginBuildingConstructionReinforcement
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class BuilderReinforcement : IExternalCommand
    {

        private Document _document;
        private UIDocument _documentUI;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;

            _documentUI = uiapp.ActiveUIDocument;
            _document = _documentUI.Document;

            try
            {
                SelectionTest();
            }
            catch (Exception e)
            {
                TaskDialog.Show("Revit!", e.ToString());
                return Result.Failed;
            }


            

        

            BasicViewModel model = new MainViewWindowsViewModel(_document, this);

            MainViewWindows window = new MainViewWindows(model);
            window.Show();



            return Result.Succeeded;
        }

        #region Selectable element from documents
        

        public void SelectionTest()
        {
            ISelectionFilter selection = new ColumnSelection
                 (
                     element => element.Category.Id == new ElementId(BuiltInCategory.OST_Columns),
                     reference => reference.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_NONE
                 );

            IList<Reference> elements = _documentUI.Selection.PickObjects
                (
                    ObjectType.Element,
                    selection

                );

            foreach (var element in elements)
            {
                BuildingAnotherShape(element);
            }
        }


        public Result ExampleInDocumentation(ref string message)
        {
            try
            {
                // Get the element selection of current document.
                Selection selection = _documentUI.Selection;
                ICollection<ElementId> selectedIds = _documentUI.Selection.GetElementIds();

                if (0 == selectedIds.Count)
                {
                    // If no elements selected.
                    TaskDialog.Show("Revit", "You haven't selected any elements.");
                }
                else
                {
                    String info = "Ids of selected elements in the document are: ";
                    foreach (ElementId id in selectedIds)
                    {
                        info += "\n\t" + id.IntegerValue;
                    }

                    TaskDialog.Show("Revit", info);
                }
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public void SelectedWalls()
        {
            /*var selectedElements = _documentUI.PickElements
                (
                    element => element is Wall,
                    PickElementOptionFactory.CreateLinkDocumentationOption()
                );

            TaskDialog.Show("Count", selectedElements.Count.ToString() );*/
        }

        #endregion


        #region Filter elements to documentation
        public void CollectorMultiFilter()
        {
            // самостоятельная фильтрация.

            // Создаётся коллекция и заполняется нужными типами
            ICollection<BuiltInCategory> categories = new List<BuiltInCategory>();

            categories.Add(BuiltInCategory.OST_Floors);
            categories.Add(BuiltInCategory.OST_Walls);
            categories.Add(BuiltInCategory.OST_Columns);

            // создаётся класс для фильтрации этих типов.
            var multiCategoryFilter = new ElementMulticategoryFilter(categories);

            // сам коллектор для сбора. который принимает фильтр.
            var collector = new FilteredElementCollector(_document);

            // Применяет элементный фильтр к коллектору.
            collector.WherePasses(multiCategoryFilter);
        }

        public void LogicalAndFilter()
        {
            //Фильтр, используемый для сопоставления элементов по их классу.
            ElementClassFilter classFilter = new ElementClassFilter(typeof(FamilyInstance));

            // Фильтр, используемый для сопоставления элементов по их категории.
            ElementCategoryFilter elementCategory = new ElementCategoryFilter(BuiltInCategory.OST_Doors);

            //Фильтр, содержащий набор фильтров. Фильтр срабатывает, когда проходят все фильтры в наборе.
            LogicalAndFilter filter = new LogicalAndFilter(classFilter, elementCategory);

            // сам коллектор для сбора. который принимает фильтр.
            var collector = new FilteredElementCollector(_document);

            // Применяет элементный фильтр к коллектору.
            collector.WherePasses(filter);

            string data = "";

            foreach (var elements in collector.ToElements())
            {
                data += elements.ToString() + " ";
            }

            MessageBox.Show(data);

        }

        public void SimpleFilter()
        {

            FilteredElementCollector collector = new FilteredElementCollector(_document)
                .OfClass(typeof(FamilyInstance));


            FilteredElementCollector collectors = new FilteredElementCollector(_document)
                   .OfCategory(BuiltInCategory.OST_Columns)
                   .WhereElementIsNotElementType();

        }

        #endregion

        private void BuildingAnotherShape(Reference reference)
        {
            Element element = _document.GetElement(reference);
         
            Location pointElement = element.Location;
            LocationPoint pointer = pointElement as LocationPoint;


            XYZ normal = new XYZ(0, 0, 1);
            XYZ origin = new XYZ(pointer.Point.X, pointer.Point.Y , pointer.Point.Z);

            XYZ offesPosition1 = new XYZ
                (
                    origin.X,
                    origin.Y,
                    origin.Z + 20
                );

            XYZ offesPosition2 = new XYZ
                (
                    offesPosition1.X,
                    offesPosition1.Y + 2,
                    offesPosition1.Z + 5
                );

            XYZ offesPosition3 = new XYZ
                (
                    offesPosition2.X,
                    offesPosition2.Y,
                    offesPosition2.Z + 5
            );

            

            List<XYZ> pointers = new List<XYZ>();

            pointers.Add(origin);
            pointers.Add(offesPosition1);
            pointers.Add(offesPosition2);
            pointers.Add(offesPosition3);

            List<Line> Lines = new List<Line>();

            for (int i = 1; i < pointers.Count; i++)
            {
                Lines.Add(Line.CreateBound(pointers[i-1], pointers[i]));
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

        public void GenerateLineFromRevitExampleFromDocumentation()
        {

            // Create a few geometry lines. These lines are transaction (not in the model),
            // therefore they do not need to be created inside a document transaction.
            XYZ Point1 = XYZ.Zero;
            XYZ Point2 = new XYZ(10, 0, 0);
            XYZ Point3 = new XYZ(10, 10, 0);
            XYZ Point4 = new XYZ(0, 10, 0);

            Line geomLine1 = Line.CreateBound(Point1, Point2);
            Line geomLine2 = Line.CreateBound(Point4, Point3);
            Line geomLine3 = Line.CreateBound(Point1, Point4);

            // This geometry plane is also transaction and does not need a transaction
            XYZ origin = XYZ.Zero;
            XYZ normal = new XYZ(0, 0, 1);
            Plane geomPlane = Plane.CreateByNormalAndOrigin(normal, origin);

            // In order to a sketch plane with model curves in it, we need
            // to start a transaction because such operations modify the model.

            // All and any transaction should be enclosed in a 'using'
            // block or guarded within a try-catch-finally blocks
            // to guarantee that a transaction does not out-live its scope.
            try
            {
                Transaction transaction = new Transaction(_document);

                if (transaction.Start("Create model curves") == TransactionStatus.Started)
                {
                    // Create a sketch plane in current document
                    SketchPlane sketch = SketchPlane.Create(_document, geomPlane);

                    // Create a ModelLine elements using the geometry lines and sketch plane
                    ModelLine line1 = _document.Create.NewModelCurve(geomLine1, sketch) as ModelLine;
                    ModelLine line2 = _document.Create.NewModelCurve(geomLine2, sketch) as ModelLine;
                    ModelLine line3 = _document.Create.NewModelCurve(geomLine3, sketch) as ModelLine;

                    // Ask the end user whether the changes are to be committed or not
                    TaskDialog taskDialog = new TaskDialog("Revit");
                    taskDialog.MainContent = "Click either [OK] to Commit, or [Cancel] to Roll back the transaction.";
                    TaskDialogCommonButtons buttons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
                    taskDialog.CommonButtons = buttons;

                    if (TaskDialogResult.Ok == taskDialog.Show())
                    {
                        // For many various reasons, a transaction may not be committed
                        // if the changes made during the transaction do not result a valid model.
                        // If committing a transaction fails or is canceled by the end user,
                        // the resulting status would be RolledBack instead of Committed.
                        if (TransactionStatus.Committed != transaction.Commit())
                        {
                            TaskDialog.Show("Failure", "Transaction could not be committed");
                        }
                    }
                    else
                    {
                        transaction.RollBack();
                    }
                }
            } catch (Exception ex) 
            {
                TaskDialog.Show("Revit", ex.ToString());
            }
        }
    }
}

