using LiveFit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveFit;
using MainForm;
//Version 1.0
namespace MiscSettings
{
    public partial class MiscSet : Form
    {
        private Form2 FitForm = null;
        private Form1 CameraForm = null;
        int longExp;
        int mediumExp;
        int shortExp;
        public MiscSet(Form2 fitting, Form1 camera)
        {
            InitializeComponent();
            FitForm = fitting;
            CameraForm = camera;
            //grab current exposure settings
            int[] ExposuresSML = camera.getExposures();
            textBoxExpS.Text = (ExposuresSML[0] / 100).ToString();
            textBoxExpM.Text = (ExposuresSML[1] / 100).ToString();
            textBoxExpL.Text = (ExposuresSML[2] / 100).ToString();
            //grab current power and µm/px settings
            textBoxpxConversion.Text = fitting.pxConversion.ToString();
            textBoxLaserPower.Text = fitting.PowerLaser.ToString();

        }

        //update the quick exposure settings
        //textbox input is in ms and has to be converted to microseconds
        private void UpdateFitSettings_Click(object sender, EventArgs e)
        {
            FitForm.pxConversion = Convert.ToDouble(textBoxpxConversion.Text);
            FitForm.PowerLaser = Convert.ToDouble(textBoxLaserPower.Text);
            //do exposures and check if too long to set
            longExp = (Convert.ToInt32(textBoxExpL.Text) * 100);
            mediumExp = (Convert.ToInt32(textBoxExpM.Text) * 100);
            shortExp = (Convert.ToInt32(textBoxExpS.Text) * 100);
            //check if exposures are fine
            if (longExp > (int)9e6) { textBoxExpL.BackColor = Color.Red; return; }
            if (mediumExp > (int)9e6) { textBoxExpM.BackColor = Color.Red; return; }
            if (shortExp > (int)9e6) { textBoxExpS.BackColor = Color.Red; return; }
            CameraForm.setExposures(longExp, mediumExp, shortExp);
            this.Dispose();
        }

        private void MiscSet_Load(object sender, EventArgs e)
        {

        }

        //sets these arbitrary default values and closes the window
        private void buttonDefault_Click(object sender, EventArgs e)
        {
            CameraForm.setExposures((int)100000, (int)15000, (int)5000);
            FitForm.pxConversion = 1;
            FitForm.PowerLaser = 1e-6;
            this.Dispose();
        }
    }
}
