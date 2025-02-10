using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopSolid.Kernel.Automating;

namespace Exercise_1
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
            List<PdmObjectId> openedProjectAndLibraries =  TopSolidHost.Pdm.GetOpenProjects(true, true);
            foreach (PdmObjectId openedProject in openedProjectAndLibraries)
            {
                TopSolidHost.Pdm.CloseProject(openedProject);
            }

            //access to interface IPDM
            //looking for projects containing this specific name.
            List<PdmObjectId> projectsWithThisName = TopSolidHost.Pdm.SearchProjectByName(projectName); 

            //if found,check the name again.
            if (projectsWithThisName.Count>0)
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
    }
}
