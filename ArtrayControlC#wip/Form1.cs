using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using ArtCamSdk;
using Python.Runtime;
using LiveFit;
using System.Diagnostics;
using System.Linq;
using MiscSettings;
using ScottPlot.Renderable;
using Microsoft.Win32;
using System.Xml;
using Encoding.UTF8;


//Version 1.2 wip
//needs some cleanup but is working (peakPower may have a wrong factor/not tested)
namespace MainForm
{

    /// <summary>
    /// This is an explanation for outline of "Form1".
    /// </summary>
    /// 

	public class Form1 : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MainMenu mainMenu1;

		private byte[] m_pCapture;
		private Bitmap m_Bitmap = null;
		private int m_PreviewMode = -1;
		private CArtCam m_CArtCam = new CArtCam();
		private int m_DllType = -1;
		private int m_DllCount = 0;
		private int m_DllSata = -1;
		private int m_SataType = -1;

		//custom parts holding variables etc I need
		private Bitmap c_RefBitmap = null;
		private Int16[] c_CameraInt = null;
		private string c_RefPath = "";
		private bool c_FitInit = false;
		//I might have to make the instance right here, right now (funksoul brother)
		//may need to push this to the constructor
		private LiveFit.Form2 FitForm = null;
		//private bool delegateActive = false;
        //timer for updating fitroutine
        //readonly Stopwatch Stopwatch = Stopwatch.StartNew();
		private int longExp = 100000;
        private int mediumExp = 15000;
        private int shortExp = 6000;
		//camera frame quick switch
		//supposed to help find beam quicker
		//width, height, FPS
		//int[] targetFrameSize = new int[3];
		//bool targetFrameBool = true;


        private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuSave;
		private System.Windows.Forms.MenuItem menuExit;
		private System.Windows.Forms.MenuItem menuView;
		private System.Windows.Forms.MenuItem menuPreview;
		private System.Windows.Forms.MenuItem menuCallback;
		private System.Windows.Forms.MenuItem menuSnapshot;
		private System.Windows.Forms.MenuItem menuCapture;
		private System.Windows.Forms.MenuItem menuTrigger;
		private System.Windows.Forms.MenuItem menuSet;
		private System.Windows.Forms.MenuItem menuCamera;
		private System.Windows.Forms.MenuItem menuFilter;
		private System.Windows.Forms.MenuItem menuAnalog;
		private System.Windows.Forms.MenuItem menuDevice;
		private System.Windows.Forms.MenuItem menuDevice0;
		private System.Windows.Forms.MenuItem menuDevice1;
		private System.Windows.Forms.MenuItem menuDevice2;
		private System.Windows.Forms.MenuItem menuDevice3;
		private System.Windows.Forms.MenuItem menuDevice4;
		private System.Windows.Forms.MenuItem menuDevice5;
		private System.Windows.Forms.MenuItem menuDevice6;
		private System.Windows.Forms.MenuItem menuDevice7;
		private System.Windows.Forms.MenuItem menuDLL;
		private System.Windows.Forms.MenuItem menuDllReload;
		private System.Windows.Forms.Panel ImagePanel;
		private System.Windows.Forms.PictureBox ImageBox;
		private MenuItem menuOpenRef;
        private MenuItem menuItemReference;
        private MenuItem menuItemSwitchExp;
        private MenuItem menuItemExpM;
        private MenuItem menuItemExpL;
        private MenuItem menuItemExpS;
        private MenuItem menuItem1;
        private MenuItem menuMiscSettings;
        private MenuItem menuItemBackground;
        private System.Windows.Forms.Timer timer1;


		[DllImport("user32.dll")]
		public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

		[DllImport("user32.dll")]
		public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);


		public Form1()
		{
			//
			// This is needed for Windows form designer support.
			//
			InitializeComponent();

			//
			// TODO: Add constructor code after calling InitializeComponent.
			//
			//I don't think this is a good idea, but I cannot remember the proper way right now
			FitForm = new LiveFit.Form2(this);
            FitForm.data_height = this.getHeight();
            FitForm.data_width = this.getWidth();
            //here the height and width still are 0?
            //fitroutine.resetXValues();
            //fitroutine.ShowDialog();

            //open to give settings option
            //MiscSettings.Form3 tempMenu = new MiscSettings.Form3(fitroutine);

            //grab current target frame size
            /*
			targetFrameSize[0] = m_CArtCam.Width();
            targetFrameSize[1] = m_CArtCam.Height();
			targetFrameSize[2] = m_CArtCam.Fps();*/
        }

		/// <summary>
		/// Execute after-treatment to resource used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
			Release();
			m_CArtCam.FreeLibrary();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// This method is required for designer support.
		/// Do not change the content of this method by code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuFile = new System.Windows.Forms.MenuItem();
            this.menuSave = new System.Windows.Forms.MenuItem();
            this.menuExit = new System.Windows.Forms.MenuItem();
            this.menuOpenRef = new System.Windows.Forms.MenuItem();
            this.menuItemReference = new System.Windows.Forms.MenuItem();
            this.menuItemBackground = new System.Windows.Forms.MenuItem();
            this.menuView = new System.Windows.Forms.MenuItem();
            this.menuPreview = new System.Windows.Forms.MenuItem();
            this.menuCallback = new System.Windows.Forms.MenuItem();
            this.menuSnapshot = new System.Windows.Forms.MenuItem();
            this.menuCapture = new System.Windows.Forms.MenuItem();
            this.menuTrigger = new System.Windows.Forms.MenuItem();
            this.menuSet = new System.Windows.Forms.MenuItem();
            this.menuCamera = new System.Windows.Forms.MenuItem();
            this.menuFilter = new System.Windows.Forms.MenuItem();
            this.menuAnalog = new System.Windows.Forms.MenuItem();
            this.menuMiscSettings = new System.Windows.Forms.MenuItem();
            this.menuDLL = new System.Windows.Forms.MenuItem();
            this.menuDllReload = new System.Windows.Forms.MenuItem();
            this.menuDevice = new System.Windows.Forms.MenuItem();
            this.menuDevice0 = new System.Windows.Forms.MenuItem();
            this.menuDevice1 = new System.Windows.Forms.MenuItem();
            this.menuDevice2 = new System.Windows.Forms.MenuItem();
            this.menuDevice3 = new System.Windows.Forms.MenuItem();
            this.menuDevice4 = new System.Windows.Forms.MenuItem();
            this.menuDevice5 = new System.Windows.Forms.MenuItem();
            this.menuDevice6 = new System.Windows.Forms.MenuItem();
            this.menuDevice7 = new System.Windows.Forms.MenuItem();
            this.menuItemSwitchExp = new System.Windows.Forms.MenuItem();
            this.menuItemExpS = new System.Windows.Forms.MenuItem();
            this.menuItemExpM = new System.Windows.Forms.MenuItem();
            this.menuItemExpL = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ImagePanel = new System.Windows.Forms.Panel();
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.ImagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFile,
            this.menuView,
            this.menuSet,
            this.menuDLL,
            this.menuDevice,
            this.menuItemSwitchExp});
            // 
            // menuFile
            // 
            this.menuFile.Index = 0;
            this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuSave,
            this.menuExit,
            this.menuOpenRef,
            this.menuItemReference,
            this.menuItemBackground});
            this.menuFile.Text = "File(&F)";
            // 
            // menuSave
            // 
            this.menuSave.Index = 0;
            this.menuSave.Text = "Save(&S)";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuExit
            // 
            this.menuExit.Index = 1;
            this.menuExit.Text = "End(&X)";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuOpenRef
            // 
            this.menuOpenRef.Index = 2;
            this.menuOpenRef.Text = "Open";
            this.menuOpenRef.Click += new System.EventHandler(this.menuOpenRef_Click);
            // 
            // menuItemReference
            // 
            this.menuItemReference.Index = 3;
            this.menuItemReference.Text = "Reference";
            this.menuItemReference.Click += new System.EventHandler(this.menuItemReference_Click);
            // 
            // menuItemBackground
            // 
            this.menuItemBackground.Index = 4;
            this.menuItemBackground.Text = "Background";
            this.menuItemBackground.Click += new System.EventHandler(this.menuItemBackground_Click);
            // 
            // menuView
            // 
            this.menuView.Index = 1;
            this.menuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuPreview,
            this.menuCallback,
            this.menuSnapshot,
            this.menuCapture,
            this.menuTrigger});
            this.menuView.Text = "Display(&V)";
            // 
            // menuPreview
            // 
            this.menuPreview.Index = 0;
            this.menuPreview.Text = "Preview(&P)";
            this.menuPreview.Click += new System.EventHandler(this.menuPreview_Click);
            // 
            // menuCallback
            // 
            this.menuCallback.Index = 1;
            this.menuCallback.Text = "Callback(&B)";
            this.menuCallback.Click += new System.EventHandler(this.menuCallback_Click);
            // 
            // menuSnapshot
            // 
            this.menuSnapshot.Index = 2;
            this.menuSnapshot.Text = "Snapshot(&S)";
            this.menuSnapshot.Click += new System.EventHandler(this.menuSnapshot_Click);
            // 
            // menuCapture
            // 
            this.menuCapture.Index = 3;
            this.menuCapture.Text = "Capture(&C)";
            this.menuCapture.Click += new System.EventHandler(this.menuCapture_Click);
            // 
            // menuTrigger
            // 
            this.menuTrigger.Index = 4;
            this.menuTrigger.Text = "Trigger(&T)";
            this.menuTrigger.Click += new System.EventHandler(this.menuTrigger_Click);
            // 
            // menuSet
            // 
            this.menuSet.Index = 2;
            this.menuSet.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuCamera,
            this.menuFilter,
            this.menuAnalog,
            this.menuMiscSettings});
            this.menuSet.Text = "Settings(&S)";
            // 
            // menuCamera
            // 
            this.menuCamera.Index = 0;
            this.menuCamera.Text = "Camera settings(&C)";
            this.menuCamera.Click += new System.EventHandler(this.menuCamera_Click);
            // 
            // menuFilter
            // 
            this.menuFilter.Index = 1;
            this.menuFilter.Text = "Filter settings(&F)";
            this.menuFilter.Click += new System.EventHandler(this.menuFilter_Click);
            // 
            // menuAnalog
            // 
            this.menuAnalog.Index = 2;
            this.menuAnalog.Text = "Analog settings(&A)";
            this.menuAnalog.Click += new System.EventHandler(this.menuAnalog_Click);
            // 
            // menuMiscSettings
            // 
            this.menuMiscSettings.Index = 3;
            this.menuMiscSettings.Text = "Misc";
            this.menuMiscSettings.Click += new System.EventHandler(this.menuMiscSettings_Click);
            // 
            // menuDLL
            // 
            this.menuDLL.Index = 3;
            this.menuDLL.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDllReload});
            this.menuDLL.Text = "DLL(&L)";
            // 
            // menuDllReload
            // 
            this.menuDllReload.Index = 0;
            this.menuDllReload.Text = "Reload";
            this.menuDllReload.Click += new System.EventHandler(this.menuDllReload_Click);
            // 
            // menuDevice
            // 
            this.menuDevice.Index = 4;
            this.menuDevice.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDevice0,
            this.menuDevice1,
            this.menuDevice2,
            this.menuDevice3,
            this.menuDevice4,
            this.menuDevice5,
            this.menuDevice6,
            this.menuDevice7});
            this.menuDevice.Text = "Device(&D)";
            // 
            // menuDevice0
            // 
            this.menuDevice0.Index = 0;
            this.menuDevice0.Text = "0";
            this.menuDevice0.Click += new System.EventHandler(this.menuDevice0_Click);
            // 
            // menuDevice1
            // 
            this.menuDevice1.Index = 1;
            this.menuDevice1.Text = "1";
            this.menuDevice1.Click += new System.EventHandler(this.menuDevice1_Click);
            // 
            // menuDevice2
            // 
            this.menuDevice2.Index = 2;
            this.menuDevice2.Text = "2";
            this.menuDevice2.Click += new System.EventHandler(this.menuDevice2_Click);
            // 
            // menuDevice3
            // 
            this.menuDevice3.Index = 3;
            this.menuDevice3.Text = "3";
            this.menuDevice3.Click += new System.EventHandler(this.menuDevice3_Click);
            // 
            // menuDevice4
            // 
            this.menuDevice4.Index = 4;
            this.menuDevice4.Text = "4";
            this.menuDevice4.Click += new System.EventHandler(this.menuDevice4_Click);
            // 
            // menuDevice5
            // 
            this.menuDevice5.Index = 5;
            this.menuDevice5.Text = "5";
            this.menuDevice5.Click += new System.EventHandler(this.menuDevice5_Click);
            // 
            // menuDevice6
            // 
            this.menuDevice6.Index = 6;
            this.menuDevice6.Text = "6";
            this.menuDevice6.Click += new System.EventHandler(this.menuDevice6_Click);
            // 
            // menuDevice7
            // 
            this.menuDevice7.Index = 7;
            this.menuDevice7.Text = "7";
            this.menuDevice7.Click += new System.EventHandler(this.menuDevice7_Click);
            // 
            // menuItemSwitchExp
            // 
            this.menuItemSwitchExp.Index = 5;
            this.menuItemSwitchExp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemExpS,
            this.menuItemExpM,
            this.menuItemExpL});
            this.menuItemSwitchExp.Text = "SwitchExp";
            // 
            // menuItemExpS
            // 
            this.menuItemExpS.Index = 0;
            this.menuItemExpS.Text = "Short";
            this.menuItemExpS.Click += new System.EventHandler(this.menuItemExpS_Click);
            // 
            // menuItemExpM
            // 
            this.menuItemExpM.Index = 1;
            this.menuItemExpM.Text = "Medium";
            this.menuItemExpM.Click += new System.EventHandler(this.menuItemExpM_Click);
            // 
            // menuItemExpL
            // 
            this.menuItemExpL.Index = 2;
            this.menuItemExpL.Text = "Long";
            this.menuItemExpL.Click += new System.EventHandler(this.menuItemExpL_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ImagePanel
            // 
            this.ImagePanel.AutoScroll = true;
            this.ImagePanel.BackColor = System.Drawing.SystemColors.Control;
            this.ImagePanel.Controls.Add(this.ImageBox);
            this.ImagePanel.Location = new System.Drawing.Point(48, 35);
            this.ImagePanel.Name = "ImagePanel";
            this.ImagePanel.Size = new System.Drawing.Size(552, 433);
            this.ImagePanel.TabIndex = 0;
            this.ImagePanel.Visible = false;
            // 
            // ImageBox
            // 
            this.ImageBox.Location = new System.Drawing.Point(16, 17);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(40, 44);
            this.ImageBox.TabIndex = 0;
            this.ImageBox.TabStop = false;
            this.ImageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageBox_Paint);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(640, 481);
            this.Controls.Add(this.ImagePanel);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Artray Controlled";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ImagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// Main entry point for application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.Run(new Form1());
		}
		 
		protected override unsafe void WndProc(ref System.Windows.Forms.Message m)
		{
			// WM_GRAPHNOTIFY
			if (DLL_MESSAGE.WM_GRAPHNOTIFY == (DLL_MESSAGE)m.Msg)
			{
			}
			// WM_ERROR
			else if (DLL_MESSAGE.WM_ERROR == (DLL_MESSAGE)m.Msg ||
				(DLL_MESSAGE.WM_GRAPHPAINT == (DLL_MESSAGE)m.Msg && null == (GP_INFO*)m.WParam)
				)
			{
			}
			// WM_GRAPHPAINT
			else if (DLL_MESSAGE.WM_GRAPHPAINT == (DLL_MESSAGE)m.Msg)
			{
				GP_INFO* pInfo = (GP_INFO*)m.WParam;
				if (pInfo != null)
				{
					ImageBox.Invalidate();
				}

			}
			else
			{
				base.WndProc(ref m);
			}
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			// Drawing by double buffer(not to flicker)
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

			if (File.Exists("Sample.xml"))
			{
				System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(int[]));
				System.IO.FileStream fs = new System.IO.FileStream("Sample.xml", System.IO.FileMode.Open);
				int[] Type = new int[2];
				Type = (int[])ser.Deserialize(fs);
				fs.Close();
				m_DllType = Type[0];
				m_SataType = Type[1];
			}
			OnDllReload();
			if (-1 != m_DllType) {
				OnDllChange((object)0, System.EventArgs.Empty, m_DllType, m_SataType);
			}
        }

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (m_DllType != -1)
			{
				int[] Type = { (int)m_DllType, (int)m_SataType };
				System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(int[]));
				System.IO.FileStream fs = new System.IO.FileStream("Sample.xml", System.IO.FileMode.Create);
				ser.Serialize(fs, Type);
				fs.Close();
			}

			Release();
		}

		private void Release()
		{
			m_CArtCam.Release();

			if (m_Bitmap != null)
			{
				m_Bitmap.Dispose();
				m_Bitmap = null;
			}

			if (c_RefBitmap != null)
			{
				c_RefBitmap.Dispose();
				c_RefBitmap = null;
			}

			m_PreviewMode = -1;
			timer1.Enabled = false;
		}


		// Save
		//this does not seem functional? at least no dialogue for saving pops up and I cannot find any files
		//saving everything myself and as a monochrome file
		private void menuSave_Click(object sender, System.EventArgs e)
		{
			if (!m_CArtCam.IsInit())
			{
				MessageBox.Show("Select available device");
				return;
			}
            //MessageBox.Show("Still to be implemented");
            /*
            Bitmap saveBitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format48bppRgb);
			//doesn'T work: this.DrawImage(saveBitmap);
			*/
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
				string filepath = null;
                saveFileDialog.InitialDirectory = "c:\\";
                saveFileDialog.Filter = "RAW data files (*.txt*)|*.txt";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filepath = saveFileDialog.FileName;
					saveRAWdata(filepath);

                }

            }

            
			//m_CArtCam.SaveImage("image.png", FILETYPE.FILETYPE_PNG);
			//m_CArtCam.SaveImage("image.RAW", FILETYPE.FILETYPE_RAW);


		}

		// End
		private void menuExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		// Preview Draw automatically
		private void menuPreview_Click(object sender, System.EventArgs e)
		{
			if (!m_CArtCam.IsInit())
			{
				MessageBox.Show("Select available device");
				return;
			}

			ImagePanel.Visible = false;
            //default is false
            timer1.Enabled = false;
			

			// Release device
			m_CArtCam.Close();

			// Set window to be displayed
			// When setting NULL to hWnd,it is possible to create new window and show it.
			m_CArtCam.SetPreviewWindow(this.Handle, 0, 0, this.Width, this.Height);

			m_CArtCam.Preview();

			// Check menu
			menuPreview.Checked = true;
			menuCallback.Checked = false;
			menuCapture.Checked = false;
			menuTrigger.Checked = false;

			m_PreviewMode = 0;
			ImageBox.Invalidate();
            //timer1.Enabled = false;
        }

		// Callback: Obtain image pointer of image and draw its own.
		private void menuCallback_Click(object sender, System.EventArgs e)
		{
			if (!m_CArtCam.IsInit())
			{
				MessageBox.Show("Select available device");
				return;
			}

			ImagePanel.Visible = true;
            timer1.Interval = 400;
            //default = false
            timer1.Enabled = true;
			
			// Release device
			m_CArtCam.Close();

			// If drawing by yourself, set all window size to 0.
			// An automatic display can be performed, if window size is set up even when using CallBackPreview
			m_CArtCam.SetPreviewWindow(this.Handle, 0, 0, 0, 0);

			// Create bit-map
			CreateBitmap();

			ImageBox.SetBounds(0, 0, getWidth(), getHeight());

			m_CArtCam.CallBackPreview(Handle, m_pCapture, getSize(), 1);

			// Check menu
			menuPreview.Checked = false;
			menuCallback.Checked = true;
			menuCapture.Checked = false;
			menuTrigger.Checked = false;

			m_PreviewMode = 1;
			ImageBox.Invalidate();
            timer1.Interval = 100;

        }

		// Snapshot
		private void menuSnapshot_Click(object sender, System.EventArgs e)
		{
			if (!m_CArtCam.IsInit())
			{
				MessageBox.Show("Select available device");
				return;
			}

			ImagePanel.Visible = true;
			timer1.Enabled = false;

			// Release device
			m_CArtCam.Close();

			// Create bit-map
			CreateBitmap();

			ImageBox.SetBounds(0, 0, getWidth(), getHeight());
			m_CArtCam.SnapShot(m_pCapture, getSize(), 1);

			// Check menu
			menuPreview.Checked = false;
			menuCallback.Checked = false;
			menuCapture.Checked = false;
			menuTrigger.Checked = false;

			m_PreviewMode = 2;
			timer1.Enabled = true;
			timer1.Interval = 100;

			ImageBox.Invalidate();
            //now forcing a refresh with this for trouble shooting reasons
            //don't see a reason to not keep this in though
            FitForm.CamIntMap = CameraIntMap;
            FitForm.RefreshImage();
		}

		// Capture
		private void menuCapture_Click(object sender, System.EventArgs e)
		{
			if (!m_CArtCam.IsInit())
			{
				MessageBox.Show("Select available device");
				return;
			}

			ImagePanel.Visible = true;
			timer1.Enabled = false;

			// Release device
			m_CArtCam.Close();

			// Create bit-map
			CreateBitmap();

			ImageBox.SetBounds(0, 0, getWidth(), getHeight());

			m_CArtCam.Capture();

			// Check menu
			menuPreview.Checked = false;
			menuCallback.Checked = false;
			menuCapture.Checked = true;
			menuTrigger.Checked = false;

			m_PreviewMode = 3;
			ImageBox.Invalidate();

			timer1.Interval = 100;
			timer1.Enabled = true;
		}

		// Capture timer
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if (m_PreviewMode == 1)
			{
				FitForm.CamIntMap = CameraIntMap;
				FitForm.RefreshImage();
			}
			if (m_PreviewMode == 3)
			{
				m_CArtCam.SnapShot(m_pCapture, getSize(), 1);
            }
			
			ImageBox.Invalidate();
        }

		

    // Trigger
    private void menuTrigger_Click(object sender, System.EventArgs e)
		{
			if (!m_CArtCam.IsInit())
			{
				MessageBox.Show("Select available device");
				return;
			}

			ImagePanel.Visible = true;
			timer1.Enabled = false;

			// Release device
			m_CArtCam.Close();

			// If drawing by yourself, set all window size to 0.
			// An automatic display can be performed, if window size is set up even when using CallBackPreview
			m_CArtCam.SetPreviewWindow(this.Handle, 0, 0, 0, 0);

			// Create bit-map
			CreateBitmap();

			ImageBox.SetBounds(0, 0, getWidth(), getHeight());
			m_CArtCam.Trigger(this.Handle, m_pCapture, getSize(), 1);

			// Check menu
			menuPreview.Checked = false;
			menuCallback.Checked = false;
			menuCapture.Checked = false;
			menuTrigger.Checked = true;

			ImageBox.Invalidate();
			m_PreviewMode = 4;

			ImageBox.Invalidate();
		}

		// Create bit-map
		private void CreateBitmap()
		{
			// In case bitmap is already created, release.
			if (null != m_Bitmap)
			{
				m_Bitmap.Dispose();
			}

			switch (getColorMode())
			{
				case 8:
				case 16:
					m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format8bppIndexed);

					// Pallet modification
					ColorPalette pal = m_Bitmap.Palette;
					Color[] cpe = m_Bitmap.Palette.Entries;

					for (int i = 0; i < 256; i++)
					{
						cpe.SetValue(Color.FromArgb(i, i, i), i);
						pal.Entries[i] = cpe[i];
					}
					m_Bitmap.Palette = pal;
					break;

				case 24: m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format24bppRgb); break;
				case 32: m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format24bppRgb); break;
				case 48: m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format24bppRgb); break;
				case 64: m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format24bppRgb); break;
			}

			// Arrangement for capture
			m_pCapture = new Byte[getSize()];
		}




		// Camera settings
		private void menuCamera_Click(object sender, System.EventArgs e)
		{
			if (!m_CArtCam.IsInit())
			{
				MessageBox.Show("Select available device");
				return;
			}

			if (0 != m_CArtCam.SetCameraDlg(this.Handle))
			{
				FitForm.data_height = this.getHeight();
				FitForm.data_width = this.getWidth();
				FitForm.ResetXValues();
				//MessageBox.Show("Bayermode: " + m_CArtCam.GetBayerMode().ToString() + "; Saturation" + m_CArtCam.GetSaturation().ToString());
				//automatic bayermode 3 apparently?
				switch (m_PreviewMode)
				{
					case 0: menuPreview_Click(sender, e); break;
					case 1: menuCallback_Click(sender, e); break;
					case 3: menuCapture_Click(sender, e); break;
					case 4: menuTrigger_Click(sender, e); break;
				}
				//handover maybe new frame positions
				this.RelayFramePosition();
			}
		}

		// Filter settings
		private void menuFilter_Click(object sender, System.EventArgs e)
		{
			if (!m_CArtCam.IsInit())
			{
				MessageBox.Show("Select available device");
				return;
			}

			m_CArtCam.SetImageDlg(this.Handle);
		}

		// Analog settings
		private void menuAnalog_Click(object sender, System.EventArgs e)
		{
			if (!m_CArtCam.IsInit())
			{
				MessageBox.Show("Select available device");
				return;
			}

			m_CArtCam.SetAnalogDlg(this.Handle);
		}

		private void OnDllReload()
		{
			// Delete DLL list
			for (int i = 0; i < m_DllCount; i++) {
				menuDLL.MenuItems.RemoveAt(1);
			}
			m_DllCount = 0;
			m_DllSata = -1;

			// Search for DLL
			String szDirPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			String[] files = Directory.GetFiles(szDirPath, "Art*.dll");
			foreach (String szFileName in files)
			{
				CArtCam ArtCam = new CArtCam();
				if (ArtCam.LoadLibrary(szFileName)) {

					// Obtain version and show it
					long ver = ArtCam.GetDllVersion() & 0xFFFF;
					String szMenu = String.Format("{0}\tVersion {1:D4}", Path.GetFileNameWithoutExtension(szFileName), ver);

					// Add to menu
					m_DllCount++;
					if (ARTCAM_CAMERATYPE.ARTCAM_CAMERATYPE_SATA == (ARTCAM_CAMERATYPE)(ArtCam.GetDllVersion() >> 16)) {

						MenuItem menuSata = new MenuItem();
						menuSata.Text = szMenu;
						String[] CameraName = {
											"LVDS",
											"300MI",
											"500MI",
											"MV413",
											"800MI",
											"036MI",
											"150P3",
											"267KY",
											"274KY",
											"625KY",
											"130MI",
											"200MI",
										};
						for (int i = 0; i < CameraName.Length; i++) {
							MenuItem mi = new MenuItem();
							mi.Text = CameraName[i];
							mi.Click += new System.EventHandler(OnMenuDLLSelect);
							menuSata.MenuItems.Add(i, mi);
						}
						menuDLL.MenuItems.Add(m_DllCount, menuSata);
						m_DllSata = m_DllCount - 1;
					} else {
						MenuItem mi = new MenuItem();
						mi.Text = szMenu;
						mi.Click += new System.EventHandler(OnMenuDLLSelect);
						menuDLL.MenuItems.Add(m_DllCount, mi);
					}
				}
			}
		}

		private void OnMenuDLLSelect(object sender, System.EventArgs e)
		{
			int id = menuDLL.MenuItems.IndexOf((MenuItem)sender);
			if (id > -1) {
				OnDllChange(sender, e, id - 1, -1);
			} else {
				int type = ((MenuItem)sender).Index;
				if ((int)ARTCAM_CAMERATYPE_SATA.ARTCAM_CAMERATYPE_SATA_LVDS <= type && type <= (int)ARTCAM_CAMERATYPE_SATA.ARTCAM_CAMERATYPE_SATA_200MI) {
					OnDllChange(sender, e, m_DllSata, type);
				}
			}
		}
		// Change Device
		private void DeviceChange(object sender, System.EventArgs e, int Number)
		{
			if (m_CArtCam.IsInit())
			{
				m_CArtCam.Close();
			}


			// To confirm whether the device is connected, use "GetDeviceName"
			// It can be found out easily with "GetDeviceName".
			// When area for obtain name is not secured, it results in error. Prepare alignment length of at least 32.
			StringBuilder Temp = new StringBuilder(256);
			if (0 == m_CArtCam.GetDeviceName(Number, Temp, 256))
			{
				m_PreviewMode = -1;
				return;
			}


			// A device will be changed, if a camera is displayed after changing the number of a device now
			// Notes: A device is not changed in this function simple substance
			//   After calling this function, a device is changed by initializing a device
			m_CArtCam.SetDeviceNumber(Number);


			for (int i = 0; i < 8; i++)
			{
				menuDevice.MenuItems[i].Checked = false;
			}
			menuDevice.MenuItems[Number].Checked = true;
		}

		private void menuDevice0_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 0); }
		private void menuDevice1_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 1); }
		private void menuDevice2_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 2); }
		private void menuDevice3_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 3); }
		private void menuDevice4_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 4); }
		private void menuDevice5_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 5); }
		private void menuDevice6_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 6); }
		private void menuDevice7_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 7); }
		private void menuDevice8_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 8); }
		private void menuDevice9_Click(object sender, System.EventArgs e) { DeviceChange(sender, e, 9); }

		private int getSize()
		{
			return ((getWidth() * (getColorMode() / 8) + 3) & ~3) * getHeight();
		}

		private int getWidth()
		{
			int[] Size = { 1, 2, 4, 8 };
			return m_CArtCam.Width() / Size[(int)(getSubSample())];
		}

		private int getHeight()
		{
			int[] Size = { 1, 2, 4, 8 };
			return m_CArtCam.Height() / Size[(int)getSubSample()];
		}

		private int getColorMode()
		{
			return ((m_CArtCam.GetColorMode() + 7) & ~7);
		}

		private int getSubSample()
		{
			return ((int)m_CArtCam.GetSubSample() & 0x03);
		}

		private BitmapData LockBitmap()
		{
			switch (getColorMode())
			{
				case 8:
				case 16:
					return m_Bitmap.LockBits(new Rectangle(0, 0, getWidth(), getHeight()), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

				case 24:
				case 32:
				case 48:
				case 64:
					return m_Bitmap.LockBits(new Rectangle(0, 0, getWidth(), getHeight()), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			}

			return null;
		}

		private void DrawImage(Graphics g)
		{
			if (null == m_Bitmap)
			{
				return;
			}

			BitmapData pBitmapData = LockBitmap();
			if (null == pBitmapData)
			{
				return;
			}


			unsafe
			{
				byte* pdest = (byte*)pBitmapData.Scan0.ToPointer();

				// In case of 16 bit transfer,convert to 8 bit and display
				if (getColorMode() == 16)
				{
					int size = getWidth() * getHeight();
					switch (m_CArtCam.GetColorMode())
					{
						case 10: for (int i = 0; i < size; i++) pdest[i] = (byte)((m_pCapture[i * 2 + 1] << 6) | (m_pCapture[i * 2] >> 2)); break;
						case 12: for (int i = 0; i < size; i++) pdest[i] = (byte)((m_pCapture[i * 2 + 1] << 4) | (m_pCapture[i * 2] >> 4)); break;
						case 14: for (int i = 0; i < size; i++) pdest[i] = (byte)((m_pCapture[i * 2 + 1] << 2) | (m_pCapture[i * 2] >> 6)); break;
						case 16: for (int i = 0; i < size; i++) pdest[i] = (byte)((m_pCapture[i * 2 + 1] << 0) | (m_pCapture[i * 2] >> 8)); break;
					}
				}
				else if (getColorMode() == 32)
				{
					int size = getWidth() * getHeight();
					for (int i = 0; i < size; i++)
					{
						pdest[i * 3] = m_pCapture[i * 4];
						pdest[i * 3 + 1] = m_pCapture[i * 4 + 1];
						pdest[i * 3 + 2] = m_pCapture[i * 4 + 2];
					}
				}
				// This is a heavy load. When using 16 (10) bit color, use C language.
				else if (getColorMode() == 48 || getColorMode() == 64)
				{
					int bpp = getColorMode() / 8;
					int size = getWidth() * getHeight();
					switch (m_CArtCam.GetColorMode())
					{
						case 42: case 58:
							for (int i = 0; i < size; i++)
							{
								pdest[i * 3] = (byte)((m_pCapture[i * bpp + 1] << 6) | (m_pCapture[i * bpp] >> 2));
								pdest[i * 3 + 1] = (byte)((m_pCapture[i * bpp + 3] << 6) | (m_pCapture[i * bpp + 2] >> 2));
								pdest[i * 3 + 2] = (byte)((m_pCapture[i * bpp + 5] << 6) | (m_pCapture[i * bpp + 4] >> 2));
							}
							break;
						case 44: case 60:
							for (int i = 0; i < size; i++)
							{
								pdest[i * 3] = (byte)((m_pCapture[i * bpp + 1] << 4) | (m_pCapture[i * bpp] >> 4));
								pdest[i * 3 + 1] = (byte)((m_pCapture[i * bpp + 3] << 4) | (m_pCapture[i * bpp + 2] >> 4));
								pdest[i * 3 + 2] = (byte)((m_pCapture[i * bpp + 5] << 4) | (m_pCapture[i * bpp + 4] >> 4));
							}
							break;
						case 46: case 62:
							for (int i = 0; i < size; i++)
							{
								pdest[i * 3] = (byte)((m_pCapture[i * bpp + 1] << 2) | (m_pCapture[i * bpp] >> 6));
								pdest[i * 3 + 1] = (byte)((m_pCapture[i * bpp + 3] << 2) | (m_pCapture[i * bpp + 2] >> 6));
								pdest[i * 3 + 2] = (byte)((m_pCapture[i * bpp + 5] << 2) | (m_pCapture[i * bpp + 4] >> 6));
							}
							break;
						case 48: case 64:
							for (int i = 0; i < size; i++)
							{
								pdest[i * 3] = (byte)((m_pCapture[i * bpp + 1] << 0) | (m_pCapture[i * bpp] >> 8));
								pdest[i * 3 + 1] = (byte)((m_pCapture[i * bpp + 3] << 0) | (m_pCapture[i * bpp + 2] >> 8));
								pdest[i * 3 + 2] = (byte)((m_pCapture[i * bpp + 5] << 0) | (m_pCapture[i * bpp + 4] >> 8));
							}
							break;
					}
				}
				else
				{
					int size = getSize();
					for (int i = 0; i < size; i++)
					{
						pdest[i] = m_pCapture[i];
					}
				}
			}

			m_Bitmap.UnlockBits(pBitmapData);


			// Image display
			int iWidth = m_Bitmap.Width;
			int iHeight = m_Bitmap.Height;
			g.DrawImage(m_Bitmap, new Rectangle(0, 0, getWidth(), getHeight()));

			// Draw line
			Point MousePos = this.PointToClient(Cursor.Position);
			if ((MousePos.X < 0) || (MousePos.Y < 0) || (MousePos.X >= iWidth) || (MousePos.Y >= iHeight)) return;

			Pen pen = new Pen(Color.Red, 1);
			Point Pos = new Point(MousePos.X - ImagePanel.AutoScrollPosition.X, MousePos.Y - ImagePanel.AutoScrollPosition.Y);
			g.DrawLine(pen, Pos.X, 0, Pos.X, getHeight());
			g.DrawLine(pen, 0, Pos.Y, getWidth(), Pos.Y);

		}

		private void Form1_Resize(object sender, System.EventArgs e)
		{
			ImagePanel.SetBounds(0, 0, this.ClientRectangle.Right, this.ClientRectangle.Bottom);
		}

		private void OnDllChange(object sender, System.EventArgs e, int DllType, int SataType)
		{
            //MessageBox.Show("EvenOnStartup");
			//this seems to run also on startup, so if I want to set default values this is probably the place (beware, this does not run on dll reload)
            Release();
			m_CArtCam.FreeLibrary();

			if (0 == m_DllCount) return;
			String stMenu = menuDLL.MenuItems[(int)DllType + 1].Text;
			String[] stArray = stMenu.Split('\t');
			String szDllName = String.Format("{0}.dll", stArray[0]);
			bool res = m_CArtCam.LoadLibrary(szDllName);
			if (!res) {
				MessageBox.Show("DLL is not found.\nIt may have been relocated after executing.");
				return;
			}
            
            // Initialize is to be called first
            // By setting Window Handle here, WMLERROR can be obtained
            if (!m_CArtCam.Initialize(this.Handle)) {
				MessageBox.Show("Failed to initialize SDK");
				return;
			}
			m_DllType = DllType;
			m_SataType = SataType;


			// Check menu
			for (int i = 0; i < m_DllCount; i++) {
				menuDLL.MenuItems[(int)i + 1].Checked = false;
			}
			// Select SATA camera type when use Sata.dll
			if (-1 != SataType && ARTCAM_CAMERATYPE.ARTCAM_CAMERATYPE_SATA == m_CArtCam.GetDllType()) {
				m_CArtCam.SetCameraType(SataType);
				m_CArtCam.SetDeviceNumber(0);
			} else {
				menuDLL.MenuItems[(int)DllType + 1].Checked = true;
			}


			for (int i = 0; i < 8; i++)
			{
				StringBuilder Temp = new StringBuilder(256);
				if (0 != m_CArtCam.GetDeviceName(i, Temp, 256))
				{
					menuDevice.MenuItems[i].Text = Temp.ToString();
					menuDevice.MenuItems[i].Enabled = true;
				}
				else
				{
					menuDevice.MenuItems[i].Enabled = false;
				}
			}

			DeviceChange(sender, e, 0);

			ImageBox.SetBounds(0, 0, getWidth(), getHeight());

			dllDefaults();
			FitForm.ResetXValues();
            //handover current position of frame
            this.RelayFramePosition();
        }

		private void menuDllReload_Click(object sender, System.EventArgs e)
		{
			OnDllReload();
            dllDefaults();
			FitForm.ResetXValues();
        }

		private void ImageBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawImage(e.Graphics);
		}

		private void menuItem1_Click(object sender, EventArgs e)
		{

		}

		private void menuOpenRef_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = "c:\\";
				openFileDialog.Filter = "RAW files (*.txt)|*.txt";
				openFileDialog.FilterIndex = 2;
				openFileDialog.RestoreDirectory = true;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
                    //Get the path of specified file
                    c_RefPath = openFileDialog.FileName;
                    /*
                    if (Path.GetExtension(openFileDialog.FileName)== "*.tif")
					{ 
						
						//Read the bitmap and store it for later
						Stream bitmapFileStream = new FileStream(c_RefPath, FileMode.Open, FileAccess.Read, FileShare.Read);
						var decoder = new TiffBitmapDecoder(bitmapFileStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
						BitmapSource RefMapSource = decoder.Frames[0];

						Bitmap RefBitmap = new Bitmap(RefMapSource.PixelWidth, RefMapSource.PixelHeight, PixelFormat.Format16bppGrayScale);
						BitmapData RefMapData = c_RefBitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, c_RefBitmap.Size), ImageLockMode.WriteOnly, PixelFormat.Format16bppGrayScale);
						RefMapSource.CopyPixels(Int32Rect.Empty, RefMapData.Scan0, RefMapData.Height * RefMapData.Stride, RefMapData.Stride);
						RefBitmap.UnlockBits(RefMapData);
                        c_RefBitmap = RefBitmap;
                    }
					else if (Path.GetExtension(openFileDialog.FileName) == "*.txt")
					{
						
					}

                    https://stackoverflow.com/questions/44353454/fast-loading-reading-tiff-files-in-c-sharp
                    Bitmap bmp = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, PixelFormat.Format32bppPArgb);
                    BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
                    bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
                    bmp.UnlockBits(data);
					*/

                    //just go ahead and read the bytes
                    byte[] inputByteArray = File.ReadAllBytes(c_RefPath);
					int length_ByteArray = inputByteArray.Length;

					//I need to finde a clever check for this
					//if (getHeight()*getWidth() != length_ByteArray) { MessageBox.Show("Incompatible reference image: Dimensions wrong!"); }

                    Int16[] RefIntMap = new short[getHeight() * getWidth()];
                    for (int i = 0; i < getHeight() * getWidth(); i++)
                    {
                        RefIntMap[i] = (short)(((((Int16)inputByteArray[(3 * i) * 2 + 1]) & 0b1111) << 8) | (inputByteArray[(3 * i) * 2]));
                    }
                    FitForm.RefIntMap = RefIntMap;
				}

			}

		}

		public Int16[] CameraIntMap
		{
			get {
                // Assume two bytes per value, bottom 4 bits from byte1 and all bits from byte0
				// however these always appear to be colour values (which are now defaulted to 48 bit RGB (no A channel) -255 saturation so they are identical)
				// only take every i % 3 == 0 value
                Int16[] IntMap = new short[getHeight()*getWidth()];
				for (int i = 0; i < getHeight() * getWidth(); i++)
				{
					IntMap[i] = (short)(((((Int16)m_pCapture[(3*i) * 2 + 1]) & 0b1111) << 8) | (m_pCapture[(3*i) * 2]));
				}
				return IntMap; }
		}
        public Int16[] RefIntMap
        {
            get
            {
				MessageBox.Show("RefIntMap deprecated");
                //ImageConverter converter = new ImageConverter();
                //return (short[])converter.ConvertTo(c_RefBitmap, typeof(short[]));
  
                return new Int16[] { 0 };
            }
        }

        private void menuItemReference_Click(object sender, EventArgs e)
        {
			if (null != m_pCapture)
			{
				FitForm.RefIntMap = CameraIntMap;
			}
        }

		private void dllDefaults()
		{
            FitForm.data_width = getWidth();
            FitForm.data_height = getHeight();
            //setting saturation to monochrome
            m_CArtCam.SetSaturation(-255);
            if (0 == m_CArtCam.SetColorMode(48)) { MessageBox.Show("Default Colour 16 bit RGB failed"); }
        }

        //GDI+ does not support 16bppGrayScale or 48bppRGB
		//Thus we pray to StackOverflow, that there some wisdom (in form of a ready made solution) may flow over
        //https://stackoverflow.com/questions/26721809/generate-16-bit-grayscale-bitmapdata-and-save-to-file
        private void save16GrayBitmap(string path)
        {
			MessageBox.Show("Deprecating so far/not tested");
            /*
			Bitmap cBitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format16bppGrayScale);
            BitmapData cBitmapData = cBitmap.LockBits(new Rectangle(0, 0, getWidth(), getHeight()), ImageLockMode.ReadWrite, PixelFormat.Format16bppGrayScale);

			int color_save = m_CArtCam.GetColorMode();
			m_CArtCam.SetColorMode(48);
			short[] intMap = new short[getWidth() * getHeight()];
            unsafe
            {
                //byte* pdest = (byte*)cBitmapData.Scan0.ToPointer();

                // This is a heavy load. When using 16 (10) bit color, use C language.
                int size = getWidth() * getHeight();
						/*
                        for (int i = 0; i < size; i++)
                        {
                            pdest[i] = (byte)((m_pCapture[i + 1] << 0) | (m_pCapture[i] >> 8));
                        }
						
						for (int i = 0; i < getHeight() * getWidth(); i++)
						{
							//pdest[i] = ((((((Int16)m_pCapture[(3 * i) * 2 + 1]) & 0b1111) << 8) | (m_pCapture[(3 * i) * 2]));
							intMap[i] = (short)(((((Int16)m_pCapture[(3 * i) * 2 + 1]) & 0b1111) << 8) | (m_pCapture[(3 * i) * 2]));
                        }
            }
				//magic stuff happens here
			var ptr = cBitmapData.Scan0;
			Marshal.Copy(intMap, 0, ptr, intMap.Length);
            
            var pixelFormats = ConvertBmpPixelFormat(PixelFormat.Format16bppGrayScale);
            BitmapSource source = BitmapSource.Create(cBitmap.Width,
                                                cBitmap.Height,
                                                cBitmap.HorizontalResolution,
            cBitmap.VerticalResolution,
                                                pixelFormats,
                                                null,
                                                cBitmapData.Scan0,
                                                cBitmapData.Stride * cBitmap.Height,
                                                cBitmapData.Stride);

            FileStream stream = new FileStream(path, FileMode.Create);

			TiffBitmapEncoder encoder = new TiffBitmapEncoder();

			encoder.Compression = TiffCompressOption.Zip;
			encoder.Frames.Add(BitmapFrame.Create(source));
			encoder.Save(stream);

			stream.Close();
			//end
            cBitmap.UnlockBits(cBitmapData);
            m_CArtCam.SetColorMode(color_save);*/
        }
        //also taken from above stackoverflow link
        private static System.Windows.Media.PixelFormat ConvertBmpPixelFormat(System.Drawing.Imaging.PixelFormat pixelformat)
        {
            System.Windows.Media.PixelFormat pixelFormats = System.Windows.Media.PixelFormats.Default;

            switch (pixelformat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
					
                    pixelFormats = System.Windows.Media.PixelFormats.Bgr32;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormats = System.Windows.Media.PixelFormats.Gray8;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    pixelFormats = System.Windows.Media.PixelFormats.Gray16;
                    break;
            }

            return pixelFormats;
        }
		private void saveRAWdata(string filepath)
		{

			File.WriteAllBytes(filepath, m_pCapture);
        }

		private ByteArray[] saveXMLdata(string filepath, string strCategory)
		{
			//taken from RelayFramePosition
			int Htotal, Hstart, Heffective, Vtotal, Vstart, Veffective;
			int[] start = new int[2];
            this.m_CArtCam.GetCaptureWindowEx(
                out Htotal, out start[(int)Form2.axis.X], out Heffective,
                out Vtotal, out start[(int)Form2.axis.Y], out Veffective);

			//let's see if I can get this to work properly
			//probably need to simplify with external function later
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.Encoding = Encoding.UTF8
			//creates with filename (file system path works)
			XmlWriter writer = XmlWriter.Create(filepath, settings);
			int year, month, day;
			writer.WriteStartDocument();
			writer.WriteStartElement("imageWrapper");
				writer.WriteStartElement("metadata");
					writer.WriteAttributeString("category", strCategory);
					writer.WriteComment("possible categories: 'measurement', 'background'");
					writer.WriteStartElement("date");
						writer.WriteElementString("year", year);
						writer.WriteElementString("month", month);
						writer.WriteElementString("day", day);
					writer.WriteEndElement();
					writer.WriteStartElement("ROI");
						writer.WriteStartElement("hstart");
							writer.WriteValue(tart[(int)Form2.axis.X]);
						writer.WriteEndElement();
						writer.WriteStartElement("hstop");
							writer.WriteValue(start[(int)Form2.axis.X]+Heffective);
						writer.WriteEndElement();
						writer.WriteStartElement("vstart");
							writer.WriteValue(start[(int)Form2.axis.Y]);
						writer.WriteEndElement();
						writer.WriteStartElement("vstop");
							writer.WriteValue(start[(int)Form2.axis.Y]+Veffective);
						writer.WriteEndElement();
					writer.WriteEndElement();
					writer.WriteStartElement("capture");
						writer.WriteStartElement("colormode");
							writer.WriteValue(this.m_CArtCam.GetColorMode());
						writer.WriteEndElement();
						writer.WriteStartElement("mirrorH");
							writer.WriteValue(this.m_CArtCam.GetMirrorV());
						writer.WriteEndElement();
						writer.WriteStartElement("mirrorV");
							writer.WriteValue(this.m_CArtCam.GetMirrorV());
						writer.WriteEndElement();
						writer.WriteStartElement("saturation");
							writer.WriteValue(this.m_CArtCam.GetSaturation());
						writer.WriteEndElement();
					writer.WriteEndElement();
					writer.WriteStartElement("camera");
						writer.WriteAttributeString("type", "ARTCAM");
						writer.WriteComment("092UV_WOM is 437 according to Artray documentation");
						//not sure if this will work
						writer.WriteValue(this.m_CArtCam.GetDllVersion() & 0xFFFF);
					writer.WriteEndElement();
				writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteStartElement("data");
				writer.WriteAttributeString("category", "base64");
				//we will see if this works or not
				//I have prepared the tissues for if it doesn't
				writer.WriteValue(Convert.ToBase64String(m_pCapture));
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();
		}

        private void menuItemRefreshScott_Click(object sender, EventArgs e)
        {
			FitForm.Refresh();
        }

        private void menuItemExpS_Click(object sender, EventArgs e)
        {
			//beware microseconds!
			m_CArtCam.SetRealExposureTime(shortExp);
			checkExp();
        }

        private void menuItemExpM_Click(object sender, EventArgs e)
        {
            m_CArtCam.SetRealExposureTime(mediumExp);
            checkExp();
        }

        private void menuItemExpL_Click(object sender, EventArgs e)
        {
            m_CArtCam.SetRealExposureTime(longExp);
            checkExp();
        }

		//checks exposure
		private void checkExp()
		{
            m_CArtCam.Close();

			// took this from snapshot_click, something here is needed for this to work
            // Create bit-map
            CreateBitmap();

            ImageBox.SetBounds(0, 0, getWidth(), getHeight());
            m_CArtCam.SnapShot(m_pCapture, getSize(), 1);

            m_PreviewMode = 2;
            ImageBox.Invalidate();
            short[] tempArray = CameraIntMap;
			if (tempArray.Max() >= 4095 * 0.95) { FitForm.BackColor = Color.Red; }
			else if (tempArray.Max() <= 4095*0.3) { FitForm.BackColor = Color.Blue; }
			else { FitForm.BackColor = Color.LightGray; }
			MessageBox.Show(tempArray.Max().ToString());
			//if (tempArray.Max() >= 4095 * 0.95) { MessageBox.Show("Warning: Overexposure"); }
        }

        private void menuMiscSettings_Click(object sender, EventArgs e)
        {
			MiscSettings.MiscSet tempMenu = new MiscSettings.MiscSet(FitForm, this);
			tempMenu.Show();
        }

		//sets exposures
		//I know this is a mess/inconsistent but it is quick
		public void setExposures(int longExposure, int mediumExposure, int shortExposure)
		{
			longExp = longExposure;
			mediumExp = mediumExposure;
			shortExp = shortExposure;
		}
		public int[] getExposures()
		{
			int[] ShortMediumLong = { shortExp, mediumExp, longExp };
			return ShortMediumLong;
		}

		//sets background from file and also sets it in registry so it automatically is found
		public void BackgroundSet()
		{
			//create/open subkey
			RegistryKey regkey;
			regkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\LiveFit", true);
			regkey.CreateSubKey("BACKGROUND_FILE");
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				string filepath = null;
				openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "RAW data files (*.txt*)|*.txt";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
                    filepath = openFileDialog.FileName;
                    regkey.SetValue("BACKGROUND_FILE", filepath);
					//have FitForm fetch the background
					FitForm.FetchBackground();
                }

				
			}
		}

		//does handover of frame starting positions
		private void RelayFramePosition()
		{
            int Htotal, Hstart, Heffective, Vtotal, Vstart, Veffective;
			int[] start = new int[2];
            this.m_CArtCam.GetCaptureWindowEx(
                out Htotal, out start[(int)Form2.axis.X], out Heffective,
                out Vtotal, out start[(int)Form2.axis.Y], out Veffective);

			FitForm.cameraFrame = start;
		}

        private void menuItemBackground_Click(object sender, EventArgs e)
        {
			BackgroundSet();
        }
    }
}



