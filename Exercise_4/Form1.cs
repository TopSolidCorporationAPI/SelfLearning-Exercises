using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopSolid.Cad.Design.Automating;
using TopSolid.Kernel.Automating;

namespace Exercise_4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btFindOrCreate_Click(object sender, EventArgs e)
        {
            //gets project name from application ui
            string projectName = this.tbProjectName.Text;

            //if textbox is empty, exit
            if (projectName.Length == 0) return;

            //checks if a project is already opened. If so, close it
            List<PdmObjectId> openedProjectAndLibraries = TopSolidHost.Pdm.GetOpenProjects(true, true);
            foreach (PdmObjectId openedProject in openedProjectAndLibraries)
            {
                TopSolidHost.Pdm.CloseProject(openedProject);
            }

            //access to interface IPDM
            //looking for projects containing this specific name.
            List<PdmObjectId> projectsWithThisName = TopSolidHost.Pdm.SearchProjectByName(projectName);

            //if found,check the name again.
            if (projectsWithThisName.Count > 0)
            {
                for (int i = 0; i < projectsWithThisName.Count; ++i)
                {
                    //gets current project being studied then fetch its name
                    PdmObjectId project = projectsWithThisName[i];
                    string currentProjectName = TopSolidHost.Pdm.GetName(project);

                    //check if name is correct
                    if (currentProjectName == projectName)
                    {
                        TopSolidHost.Pdm.OpenProject(projectsWithThisName[i]);
                        //project is opened => end application
                        return;
                    }
                }
            }

            //no project found => create a new one with this name then open it
            //Note that you can tell whether it is a working project or a library project
            PdmObjectId newProject = TopSolidHost.Pdm.CreateProject(projectName, false);

            //throw an error is project has not been created
            if (newProject.IsEmpty)
            {
                throw new Exception("An error occured!");
            }
            TopSolidHost.Pdm.OpenProject(newProject);
        }

        private void btCreateShape_Click(object sender, EventArgs e)
        {
            //get current project (we assume that first step has been done just before)
            PdmObjectId currentProject = TopSolidHost.Pdm.GetCurrentProject();

            //create a part document
            PdmObjectId partDocumentPdmId = TopSolidHost.Pdm.CreateDocument(currentProject, ".TopPrt", true);

            //gets document
            DocumentId partDocumentId = TopSolidHost.Documents.GetDocument(partDocumentPdmId);
            if (partDocumentId == DocumentId.Empty) return;

            //open document (not mandatory)
            TopSolidHost.Documents.Open(ref partDocumentId);

            //create a sketch
            CreateASketch(ref partDocumentId);

            TopSolidHost.Pdm.Save(partDocumentPdmId, true);
        }



        private void btCreateAssembly_Click(object sender, EventArgs e)
        {
            //get current project (we assume that first step has been done just before)
            PdmObjectId currentProject = TopSolidHost.Pdm.GetCurrentProject();

            //create an assembly document
            PdmObjectId assemblyDocumentPdmId = TopSolidHost.Pdm.CreateDocument(currentProject, ".TopAsm", true);

            //gets document
            DocumentId assemblyDocumentId = TopSolidHost.Documents.GetDocument(assemblyDocumentPdmId);
            if (assemblyDocumentId == DocumentId.Empty) return;

            //find previously created part document
            List<PdmObjectId> partDocumentsFound = TopSolidHost.Pdm.SearchDocumentByName(currentProject, "MyPartDocument");
            if (partDocumentsFound.Count == 0)
            {
                throw new Exception("Can't find the part document!");
            }

            //gets PdmObject found
            PdmObjectId partDocumentPdmId = partDocumentsFound.FirstOrDefault();
            if (partDocumentPdmId.IsEmpty)
            {
                throw new Exception("An error occured with the part document!");
            }

            //Gets Document 
            DocumentId partDocumentId = TopSolidHost.Documents.GetDocument(partDocumentPdmId);
            if (partDocumentId == DocumentId.Empty) return;

            //open document (not mandatory)
            TopSolidHost.Documents.Open(ref partDocumentId);

            //create a frame: add a try-catch 
            CreateAndPublishFrame(ref partDocumentId);

            //saves part document
            TopSolidHost.Pdm.Save(partDocumentPdmId, true);

            //open document (not mandatory)
            TopSolidHost.Documents.Open(ref assemblyDocumentId);

            //Open assembly document then include part document several times, with an offset
            MakeInclusionsInAssemblyDocument(ref assemblyDocumentId, partDocumentId);

            //saves assembly document
            TopSolidHost.Pdm.Save(assemblyDocumentPdmId, true);
        }

        private void btExport_Click(object sender, EventArgs e)
        {
            //get current project (we assume that previous steps have been done just before)
            PdmObjectId currentProject = TopSolidHost.Pdm.GetCurrentProject();

            //find previously created assembly document
            List<PdmObjectId> assemblyDocumentFound = TopSolidHost.Pdm.SearchDocumentByName(currentProject, "MyAssemblyDocument");
            if (assemblyDocumentFound.Count == 0)
            {
                throw new Exception("Can't find the assembly document!");
            }

            //gets PdmObject found
            PdmObjectId assemblyDocumentPdmId = assemblyDocumentFound.FirstOrDefault();
            if (assemblyDocumentPdmId.IsEmpty)
            {
                throw new Exception("An error occured with the assembly document!");
            }

            //Get Document 
            DocumentId assemblyDocumentId = TopSolidHost.Documents.GetDocument(assemblyDocumentPdmId);
            if (assemblyDocumentId == DocumentId.Empty) return;

            //get exporter index
            int exporterIndexForStl = this.GetExporterIndex(".stl");

            //check if current license can export this kind of file
            if (!TopSolidHost.Documents.CanExport(exporterIndexForStl, assemblyDocumentId)) return;

            //Show file path selection dialog dialog
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.FileName = "MyExportedAssembly";
            saveFileDialog.Filter = "STL Files (*.stl)| *.stl";
            saveFileDialog.ShowDialog();

            //check exporter index value
            if (exporterIndexForStl == TopSolidHost.Application.ExporterCount + 1)
            {
                MessageBox.Show("Cannot Export");
                return;
            }

            //export to file
            TopSolidHost.Documents.Export(exporterIndexForStl, assemblyDocumentId, saveFileDialog.FileName);
        }


        private void btImport_Click(object sender, EventArgs e)
        {
            //get current project (we assume that previous steps have been done just before)
            PdmObjectId currentProject = TopSolidHost.Pdm.GetCurrentProject();

            //get importer index
            int importerIndexForStl = this.GetImporterIndex(".stl");

            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.AddExtension = false;
            openFileDialog.Filter = "STL Files (*.stl)| *.stl";
            openFileDialog.CheckFileExists = true;
            openFileDialog.ShowDialog();

            //check importer index value
            if (importerIndexForStl == TopSolidHost.Application.ExporterCount + 1)
            {
                MessageBox.Show("Cannot Import");
                return;
            }

            //sets options
            List<KeyValue> inOptions = new List<KeyValue>();
            inOptions.Add(new KeyValue("CREATION_MODE", "SolidShape"));

            //import file
            TopSolidHost.Documents.ImportWithOptions(importerIndexForStl, inOptions, openFileDialog.FileName, currentProject, out _, out _);
        }


        #region private methods
        private static void CreateASketch(ref DocumentId partDocumentId)
        {
            // add a try-catch 
            try
            {
                //first inform TopSolid that modifications are on their way
                TopSolidHost.Application.StartModification("Create sketch into document", false);

                //Ensure document is ready to be modified (dirty)
                TopSolidHost.Documents.EnsureIsDirty(ref partDocumentId);

                //sets document name
                TopSolidHost.Documents.SetName(partDocumentId, "MyPartDocument");

                // Create a new 2D sketch that we will modify
                ElementId sketchId = TopSolidHost.Sketches2D.CreateSketchIn3D(partDocumentId,
                            new SmartPlane3D(TopSolidHost.Geometries3D.GetAbsoluteXYPlane(partDocumentId), false),
                            new SmartPoint3D(TopSolidHost.Geometries3D.GetAbsoluteOriginPoint(partDocumentId)),
                            false,
                            new SmartDirection3D(TopSolidHost.Geometries3D.GetAbsoluteYAxis(partDocumentId),
                                false));

                //modifies sketch name
                TopSolidHost.Elements.SetName(sketchId, "MySketch");

                //Start modifications of the sketch
                TopSolidHost.Sketches2D.StartModification(sketchId);

                //creates some 2D points to use to sketch a square
                Point2D pt1 = new Point2D(0, 0);
                Point2D pt2 = new Point2D(0, 0.05);
                Point2D pt3 = new Point2D(0.05, 0.05);
                Point2D pt4 = new Point2D(0.05, 0);

                //create vertex points on the sketch
                ElementItemId vertexPt1 = TopSolidHost.Sketches2D.CreateVertex(pt1);
                ElementItemId vertexPt2 = TopSolidHost.Sketches2D.CreateVertex(pt2);
                ElementItemId vertexPt3 = TopSolidHost.Sketches2D.CreateVertex(pt3);
                ElementItemId vertexPt4 = TopSolidHost.Sketches2D.CreateVertex(pt4);

                //create related line segments
                ElementItemId segment1 = TopSolidHost.Sketches2D.CreateLineSegment(vertexPt1, vertexPt2);
                ElementItemId segment2 = TopSolidHost.Sketches2D.CreateLineSegment(vertexPt2, vertexPt3);
                ElementItemId segment3 = TopSolidHost.Sketches2D.CreateLineSegment(vertexPt3, vertexPt4);
                ElementItemId segment4 = TopSolidHost.Sketches2D.CreateLineSegment(vertexPt4, vertexPt1);

                //Instanciate a list of segments with created segments
                List<ElementItemId> listOfSegments = new List<ElementItemId> { segment1, segment2, segment3, segment4 };

                //create closed profile
                TopSolidHost.Sketches2D.CreateProfile(listOfSegments);

                //Finish modifications of the sketch
                TopSolidHost.Sketches2D.EndModification();

                //now, extrude the sketch to create a shape
                TopSolidHost.Shapes.CreateExtrudedShape(partDocumentId, new SmartSection3D(sketchId), SmartDirection3D.DZ, new SmartReal(UnitType.Length, 1), new SmartReal(UnitType.Angle, 0), false, false);

                //everything ok => apply modifications
                TopSolidHost.Application.EndModification(true, true);
            }
            catch // if something went wrong, cancel modification
            {
                TopSolidHost.Application.EndModification(false, false);
            }
        }

        private static void MakeInclusionsInAssemblyDocument(ref DocumentId assemblyDocumentId, DocumentId partDocumentId)
        {
            //add a try-catch 
            try
            {
                //first inform TopSolid that modifications are on their way
                TopSolidHost.Application.StartModification("Include occurences", false);

                //Ensure document is ready to be modified (dirty)
                TopSolidHost.Documents.EnsureIsDirty(ref assemblyDocumentId);

                //sets name
                TopSolidHost.Documents.SetName(assemblyDocumentId, "MyAssemblyDocument");

                //create new frame which will be used for next code
                ElementId absoluteFrameEntity = TopSolidHost.Geometries3D.GetAbsoluteFrame(assemblyDocumentId);

                //create a new positioning before first inclusion
                ElementId positioningId = ElementId.Empty;

                //creates first inclusion at origin
                ElementId inclusion = TopSolidDesignHost.Assemblies.CreateInclusion(assemblyDocumentId, positioningId, null, partDocumentId, null, null, null, true,
                    ElementId.Empty, ElementId.Empty, false, false, false, false, Transform3D.Identity, false);
                //set number of repetitions
                int nbOfRepetitions = 3;
                double offsetDistanceValue = 0.150;
                for (int i = 1; i <= nbOfRepetitions; i++)
                {
                    //create a new positioning for new inclusion
                    positioningId = TopSolidDesignHost.Assemblies.CreatePositioning(assemblyDocumentId);

                    inclusion = TopSolidDesignHost.Assemblies.CreateInclusion(assemblyDocumentId, positioningId, null, partDocumentId, null, null, null, true,
                             ElementId.Empty, ElementId.Empty, false, false, false, false, Transform3D.Identity, false);

                    if (inclusion != ElementId.Empty)
                    {
                        //getting the first inclusion child
                        ElementId insertedElementId = TopSolidDesignHost.Assemblies.GetInclusionChildOccurrence(inclusion);

                        //looking for published frames into the occurence document
                        DocumentId instanceDocument = TopSolidDesignHost.Assemblies.GetOccurrenceDefinition(insertedElementId);
                        ElementId absoluteFrameInPartDocument = TopSolidHost.Geometries3D.GetAbsoluteFrame(instanceDocument);
                        List<ElementId> publishings = TopSolidHost.Entities.GetPublishings(instanceDocument);
                        ElementId publishedFrame = ElementId.Empty;
                        foreach (ElementId publishing in publishings)
                        {
                            if (TopSolidHost.Elements.GetTypeFullName(publishing).Contains("PublishingFrameEntity"))
                            {
                                if (TopSolidHost.Elements.GetFriendlyName(publishing) == "MyPublishedFrame")
                                {
                                    ElementId publishingFrameImage = TopSolidDesignHost.Assemblies.GetOccurrencePublishing(insertedElementId, publishing);
                                    publishedFrame = publishingFrameImage;
                                    break;
                                }
                            }
                        }

                        if (publishedFrame != ElementId.Empty)
                        {
                            SmartFrame3D frameDestination = new SmartFrame3D(absoluteFrameEntity, false);
                            SmartFrame3D frameSource = new SmartFrame3D(publishedFrame, false);

                            SmartReal offsetDistance = new SmartReal(UnitType.Length, offsetDistanceValue * (i - 1));
                            SmartReal offsetZero = new SmartReal(UnitType.Length, 0.0);
                            SmartReal rotationAngle = new SmartReal(UnitType.Angle, 0);
                            SmartReal yRotationAngle = new SmartReal(UnitType.Angle, 0);
                            SmartReal zRotationAngle = new SmartReal(UnitType.Angle, 0);

                            //add a constraint to positioning
                            TopSolidDesignHost.Assemblies.CreateFrameOnFrameConstraintWithXYOffsets(positioningId, frameSource, frameDestination, offsetDistance, false, offsetZero, false, offsetZero, false, rotationAngle,
                                false, yRotationAngle, false, zRotationAngle, false, false);
                        }
                    }
                }

                //everything ok => apply modifications
                TopSolidHost.Application.EndModification(true, true);
            }
            catch // if something went wrong, cancel modification
            {
                TopSolidHost.Application.EndModification(false, false);
            }
        }

        private static void CreateAndPublishFrame(ref DocumentId partDocumentId)
        {
            try
            {
                //first inform TopSolid that modifications are on their way
                TopSolidHost.Application.StartModification("Create frame into part document and publish it", false);

                //Ensure document is ready to be modified (dirty)
                TopSolidHost.Documents.EnsureIsDirty(ref partDocumentId);

                //create a frame, from a basic point
                Point3D point3D = new Point3D(Frame3D.OXYZ.Origin.X - 0.15, Frame3D.OXYZ.Origin.Y, Frame3D.OXYZ.Origin.Z);
                Frame3D frame3D = new Frame3D(point3D, Direction3D.DX, Direction3D.DY, Direction3D.DZ);
                ElementId frameCreated = TopSolidHost.Geometries3D.CreateFrame(partDocumentId, frame3D);

                if (frameCreated != null)
                {
                    //sets name for frame
                    TopSolidHost.Elements.SetName(frameCreated, "MyFrame");

                    //publish frame
                    ElementId publishedFrame = TopSolidHost.Geometries3D.PublishFrame(partDocumentId, "Published Frame", new SmartFrame3D(frameCreated, false));

                    //sets name for published frame
                    TopSolidHost.Elements.SetName(publishedFrame, "MyPublishedFrame");
                }

                //everything ok => apply modifications
                TopSolidHost.Application.EndModification(true, true);
            }
            catch // if something went wrong, cancel modification
            {
                TopSolidHost.Application.EndModification(false, false);
            }
        }

        #endregion

        #region Exporter / Importer methods
        private int GetExporterIndex(string wantedFileExtension)
        {
            //Set a default impossible value to the exporter index
            int exporterIndex = -1;

            //Browse all exporters to return the index of the one needed
            for (int i = 0; i < TopSolidHost.Application.ExporterCount; i++)
            {
                string fileType = "";
                string[] fileExtension = null;
                TopSolidHost.Application.GetExporterFileType(i, out fileType, out fileExtension);

                //Set the file extension in capital letter to avoid all possible problems
                List<string> extensions = fileExtension.ToList().ConvertAll(d => d.ToUpper());
                if (extensions.Contains(wantedFileExtension.ToUpper()))
                {
                    exporterIndex = i;
                    break;
                }
            }

            return exporterIndex;
        }

        private int GetImporterIndex(string wantedFileExtension)
        {
            //Set a default impossible value to the exporter index
            int importerIndex = -1;

            //Browse all exporters to return the index of the one needed
            for (int i = 0; i < TopSolidHost.Application.ImporterCount; i++)
            {
                string fileType = "";
                string[] fileExtension = null;
                TopSolidHost.Application.GetImporterFileType(i, out fileType, out fileExtension);

                //Set the file extension in capital letter to avoid all possible problems
                List<string> extensions = fileExtension.ToList().ConvertAll(d => d.ToUpper());
                if (extensions.Contains(wantedFileExtension.ToUpper()))
                {
                    importerIndex = i;
                    break;
                }
            }

            return importerIndex;
        }
        #endregion

    }
}
