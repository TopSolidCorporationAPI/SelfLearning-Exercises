using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopSolid.Kernel.Automating;

namespace Exercise_2
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

            TopSolidHost.Documents.Open(ref partDocumentId);

            //create a sketch: add a try-catch 
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

            TopSolidHost.Pdm.Save(partDocumentPdmId, true);            
        }
    }
}
