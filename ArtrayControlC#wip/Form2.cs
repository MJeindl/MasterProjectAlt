using MainForm;
using ScottPlot.Renderable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ScottPlot;
using ScottPlot.Plottable;
using static Python.Runtime.TypeSpec;
using ScottPlot.Styles;
using ArtCamSdk;
using System.Xml;
using System.Security.Cryptography;
using ScottPlot.Drawing.Colormaps;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.IO;

//Version 1.2 wip
//needs some cleanup but is working (peakPower may have a wrong factor/not tested)
namespace LiveFit
{
    public partial class Form2 : Form
    {
        private MainForm.Form1 camera;

        private short[] m_RefInt;
        private short[] m_ImageInt;
        private short[] m_BackgroundRemoveInt = new short[1280*720];
        private int m_xStart;
        private int m_yStart;

        // private Bitmap refBitmap = null;
        // private Bitmap cameraBitmap = null;

        // X is defined as along in row direction  [x,y]
        // Y is defined as along in column direction 
        public enum axis : int
        { X = 0, Y = 1 }
        //refFit[X/Y],[center,sig, amplitude, offset]
        private double[][] refFit = new double[2][];
        private double[][] cameraFit = new double[2][];

        //this should be overwritten relatively soon anyways
        private int m_dheight = 1;
        private int m_dwidth = 1;


        enum DataOrigin : int
        {
            Fit_Camera = 0,
            Fit_Ref = 1
        }

        //Define a unittype and a place to store the conversion
        double muToPx = 1;
        enum unitType : int
        {
            px = 1,
            µm = 0
        }
        //Laserpower in W
        double laserPower = 1e-6;
        

        //these are used to refresh the plot
        //if we want to, I have to add more to plot the actual data along (need to basically make a snapshot scatterplot for that)
        //What did they say? Premature optimization is the death of all code?
        //Good! This is a cemetery now!
        //I am using signalxy for performance reasons without having tested how fast scatter is just to avoid having to rewrite it all
        double[] xFitOutCam;
        double[] yFitOutCam;
        double[] xFitOutRef;
        double[] yFitOutRef;
        //live normed data of camera
        double[] xLiveOut;
        double[] yLiveOut;
        //default is false
        bool plotLive = true;


        //number of points per unit
        int sample_rate = 10;
        //for abszissae values:
        //double[][] xAxisVal = new double[2][];
        double[] xAxis = new double[5];

        
       

        //ScottPlot.Plottable.ScatterPlot[,] SignalPlot = new ScottPlot.Plottable.ScatterPlot[2,2];
        ScottPlot.Plottable.ScatterPlot xRefPlot;
        ScottPlot.Plottable.ScatterPlot yRefPlot;
        ScottPlot.Plottable.ScatterPlot xCamPlot;
        ScottPlot.Plottable.ScatterPlot yCamPlot;
        //ScottPlot.Plottable.Text textInformation;
        //ScottPlot.Plottable.ScatterPlot xLivePlot;
        //ScottPlot.Plottable.ScatterPlot yLivePlot;


        string[][] titleTemplate;

        public Form2(MainForm.Form1 camera)
        {
            InitializeComponent();
            this.camera = camera;

            //initialize plot
            //var plt = new ScottPlot.Plot(600, 400);

            ResetXValues();
            formsPlot1.Plot.Layout(top: 200);
            //plot them once
            formsPlot1.Plot.Resize(800, 800);
            xRefPlot = formsPlot1.Plot.AddScatter(xAxis, xFitOutRef, markerSize: 0, color: Color.Red);
            yRefPlot = formsPlot1.Plot.AddScatter(xAxis, yFitOutRef, markerSize: 0, color: Color.Red);
            xCamPlot = formsPlot1.Plot.AddScatter(xAxis, xFitOutCam, markerSize: 0, color: Color.Blue);
            yCamPlot = formsPlot1.Plot.AddScatter(xAxis, yFitOutCam, markerSize: 0, color: Color.Blue);
            xRefPlot.Label = "Fit of x/hor reference";
            yRefPlot.Label = "Fit of y/ver reference";
            xCamPlot.Label = "Fit of x/hor live image";
            xCamPlot.Label = "Fit of y/ver live image";

            //xLivePlot = formsPlot1.Plot.AddScatter(xAxis, xLiveOut, markerSize: 0, color: Color.Purple);
            //xLivePlot = formsPlot1.Plot.AddScatter(xAxis, yLiveOut, markerSize: 0, color: Color.Purple);

            xRefPlot.OffsetY = 0.75;
            xCamPlot.OffsetY = 0.75;
            //xLivePlot.OffsetY = 2;




            formsPlot1.Plot.AddText("x/horizontal", 0, 1, size: 16, color: Color.Blue);
            formsPlot1.Plot.AddText("y/vertical", 0, 0.25, size: 16, color: Color.Blue);


            //show frametimes
            //plt.Benchmark(enable: true);
            //new ScottPlot.FormsPlotViewer(plt).Show();


            //I am afraid I won't be able to use var sigx = Scottplot.AddSignal for this, so I have to work around this
            //this means shifting won't work?
            this.Show();
            titleTemplate = new string[2][];
            titleTemplate[0] = new string[2];
            titleTemplate[0][(int)axis.X] = "Hor/X ";
            titleTemplate[0][(int)axis.Y] = "Ver/Y ";
            titleTemplate[1] = new string[4];
            titleTemplate[1][0] = "FWHM : ";
            titleTemplate[1][1] = "Center : ";
            titleTemplate[1][2] = "Overlap : ";
            titleTemplate[1][3] = "Total Overlap : ";

            //initialize background if there was a file written
            this.FetchBackground();
        }
        /*InitPlot
        private void InitPlot()
        {
            //if (plot_init == true) { return; }
            //initialize the plot values
            resetXValues();f
            SignalPlot[(int)axis.X, (int)DataOrigin.Fit_Ref] = plt.AddScatter(xAxis, xFitOutRef);
            SignalPlot[(int)axis.Y, (int)DataOrigin.Fit_Ref] = plt.AddScatter(xAxis, yFitOutRef);
            SignalPlot[(int)axis.X, (int)DataOrigin.Fit_Camera] = plt.AddScatter(xAxis, xFitOutCam);
            SignalPlot[(int)axis.Y, (int)DataOrigin.Fit_Camera] = plt.AddScatter(xAxis, yFitOutCam);
            //plot_init = true;
        }
        */
        private void FormsPlot1_Load(object sender, EventArgs e)
        {

        }

        //refresh the image
        public void RefreshImage()
        {
            /*Forced to use the same length
            //reference fits
            if (refFit[(int)axis.X] != null) { xFitOutRef = GaussCalc(refFit[(int)DataOrigin.Fit_Ref], xAxisVal[(int)axis.X]); }
            if (refFit[(int)axis.Y] != null) { yFitOutRef = GaussCalc(refFit[(int)DataOrigin.Fit_Ref], xAxisVal[(int)axis.Y]); }
            //live camera fits
            if (cameraFit[(int)axis.X] != null) { xFitOutCam = GaussCalc(cameraFit[(int)DataOrigin.Fit_Ref], xAxisVal[(int)axis.X]); }
            if (cameraFit[(int)axis.Y] != null) { yFitOutCam = GaussCalc(cameraFit[(int)DataOrigin.Fit_Ref], xAxisVal[(int)axis.Y]); }
            */
            //reference fits
            if (refFit[(int)axis.X] != null) { xFitOutRef = GaussCalc(refFit[(int)axis.X], xAxis, true); }
            if (refFit[(int)axis.Y] != null) { yFitOutRef = GaussCalc(refFit[(int)axis.Y], xAxis, true); }
            //live camera fits
            if (cameraFit[(int)axis.X] != null) { xFitOutCam = GaussCalc(cameraFit[(int)axis.X], xAxis, true); }
            if (cameraFit[(int)axis.Y] != null) { yFitOutCam = GaussCalc(cameraFit[(int)axis.Y], xAxis, true); }


            xRefPlot.Update(xAxis, xFitOutRef);
            yRefPlot.Update(xAxis, yFitOutRef);
            xCamPlot.Update(xAxis, xFitOutCam);
            yCamPlot.Update(xAxis, yFitOutCam);

            //check if plotLive is wanted
            /*
            if (plotLive)
            {
                yLiveOut = PlottableMapSlice(CamIntMap, axis.Y, 50);
                yLivePlot.Update(xAxis, yLiveOut);
                //get the corresponding vectors
                if (cameraFit[(int)axis.X] != null)
                {
                    xLiveOut = PlottableMapSlice(CamIntMap, axis.X, (int)cameraFit[(int)axis.Y][0]);
                    xLivePlot.Update(xAxis, xLiveOut);
                }
                if (cameraFit[(int)axis.X] != null)
                {
                    yLiveOut = PlottableMapSlice(CamIntMap, axis.Y, (int)cameraFit[(int)axis.X][0]);
                    yLivePlot.Update(xAxis, yLiveOut);
                }
                
            }
            */

            formsPlot1.Plot.Title(TitleMaker(), size: 24, color: Color.MediumPurple, bold: true);

            formsPlot1.Refresh();
            //formsPlot1.Render();
            //comment this out when not testing
            TestLiveImage();
        }

        //calling this if things change in the camera menu
        public void ResetXValues()
        {
            int plot_x_length;
            if (data_height > data_width) { plot_x_length = data_height * sample_rate; }
            else { plot_x_length = data_width * sample_rate; }
            /*
            xFitOutCam = new double[plot_x_length];
            yFitOutCam = new double[plot_x_length];
            xFitOutRef = new double[plot_x_length];
            yFitOutRef = new double[plot_x_length];
            xAxis = new double[plot_x_length];
            */
            //after changing scottplot to update this doesn't matter anymore, probably can go back to simpler option
            Array.Resize<double>(ref xFitOutCam, plot_x_length);
            Array.Resize<double>(ref yFitOutCam, plot_x_length);
            Array.Resize<double>(ref xFitOutRef, plot_x_length);
            Array.Resize<double>(ref yFitOutRef, plot_x_length);

            //no liveplotting for now
            //Array.Resize<double>(ref xLiveOut, plot_x_length);
            //Array.Resize<double>(ref yLiveOut, plot_x_length);

            Array.Resize<double>(ref xAxis, plot_x_length);
            for (int i = 0; i < plot_x_length; i++) { xAxis[i] = (double)i / sample_rate; }
        }

        enum FitResult
        {
            SUCCESS,
            NoREF,
            FAILURE
        }


        //Use onedim data map to get the x and y fit 
        //subtracts the background along the way
        private double[][] FitHandler(short[] inputMap)
        {
            double[][] outFits = new double[2][];
            //gather
            //X fitting routine
            //sum over y dimension for y_sum
            double[] y_sum = new double[data_width];
            double[] x_sum = new double[data_height];
            /*
            for (int y_it = 0; y_it < data_height; y_it++)
            {
                for (int x_it = 0; x_it < data_width; x_it++)
                {
                    y_sum[x_it] += inputMap[x_it + y_it * data_width] - m_BackgroundRemoveInt[x_it + m_xStart + (y_it + m_yStart) * 1280];
                    x_sum[y_it] += inputMap[x_it + y_it * data_width] - m_BackgroundRemoveInt[x_it + m_xStart + (y_it + m_yStart) * 1280];
                }
            }
            */
            for (int y_it = 0; y_it < data_height; y_it++)
            {
                for (int x_it = data_width-1; x_it > 0; x_it--)
                {
                    //1280 + x_index - m_xStart - x_width + (y_index + m_yStart) * 1280
                    y_sum[x_it] += inputMap[x_it + y_it * data_width] 
                        - m_BackgroundRemoveInt[x_it - m_xStart - data_width + (y_it + m_yStart + 1) * 1280];
                    x_sum[y_it] += inputMap[x_it + y_it * data_width] 
                        - m_BackgroundRemoveInt[x_it - m_xStart - data_width + (y_it + m_yStart + 1) * 1280];
                }
            }
            //calculate x and y center with one gaussfit each
            double[] xtemp_Fit = FitGauss(y_sum);
            double[] ytemp_Fit = FitGauss(x_sum);
            double[] x_Row = new double[data_width];
            double[] y_Row = new double[data_height];
            double[] x_Fit = null;
            double[] y_Fit = null;

            //True x fit
            //use the right y index to get the right x slice
            /*
            if (ytemp_Fit != null)
            {
                int y_center = (int)ytemp_Fit[0];
                if ((y_center < data_height) & (y_center > 0)) 
                {
                    // I am not 100% sure which dimension to use for this (aka no clue in which direction the array is written)
                    // -> should be fine, though not tested in specifics
                    for (int x_it = 0; x_it < data_width; x_it++) 
                    { 
                        x_Row[x_it] = inputMap[x_it + y_center * data_width] 
                            - m_BackgroundRemoveInt[x_it + m_xStart + (y_center + m_yStart) * 1280]; 
                    }
                    x_Fit = FitGauss(x_Row);
                }
            }
            //True y fit
            if (xtemp_Fit != null)
            {
                int x_center = (int)xtemp_Fit[0];
                if ((x_center < data_height) & (x_center > 0))
                {
                    for (int y_it = 0; y_it < data_height; y_it++) 
                    { 
                        y_Row[y_it] = inputMap[y_it * data_width + x_center] 
                            - m_BackgroundRemoveInt[(y_it + m_yStart) * 1280 + x_center + m_xStart]; 
                    }
                    y_Fit = FitGauss(y_Row);
                }
                    
            }
            */
            //inverted version with more native m_xyStart behavior
            if (ytemp_Fit != null)
            {
                int y_center = (int)ytemp_Fit[0];
                if ((y_center < data_height) & (y_center > 0))
                {
                    // I am not 100% sure which dimension to use for this (aka no clue in which direction the array is written)
                    // -> should be fine, though not tested in specifics
                    for (int x_it = data_width-1; x_it > 0; x_it--)
                    {
                        x_Row[x_it] = inputMap[x_it + y_center * data_width]
                            - m_BackgroundRemoveInt[x_it - m_xStart -data_width + (y_center + m_yStart + 1) * 1280];
                        //1280 + x_index - m_xStart - x_width + (y_index + m_yStart) * 1280
                    }
                    x_Fit = FitGauss(x_Row);
                }
            }
            //True y fit
            if (xtemp_Fit != null)
            {
                int x_center = (int)xtemp_Fit[0];
                if ((x_center < data_height) & (x_center > 0))
                {
                    for (int y_it = 0; y_it < data_height; y_it++)
                    {
                        y_Row[y_it] = inputMap[y_it * data_width + x_center]
                            - m_BackgroundRemoveInt[(y_it + m_yStart+1) * 1280 + x_center - m_xStart - data_width];
                    }
                    y_Fit = FitGauss(y_Row);
                }

            }
            

            //x fit return
            if (x_Fit != null)
            {
                outFits[(int)axis.X] = x_Fit;
            }
            //for (int i = 0; i < 4; i++) { outFits[(int)axis.X][i] = x_Fit[i]; } }
            else { outFits[(int)axis.X] = null; }
            //y fit return
            if (y_Fit != null)
            {
                outFits[(int)axis.Y] = y_Fit;
            }
            //for (int i = 0; i < 4; i++) { outFits[(int)axis.Y][i] = y_Fit[i]; } }
            else { outFits[(int)axis.Y] = null; }
            return outFits;
        }

        //does the fit for a one dimensional array of intensity data
        private double[] FitGauss(double[] input)
        {
            if (input == null) { return null; }
            //generate position data
            //alternatively use:
            //int[] position = Enumerable.Range(0,input.Length).ToArray();
            //I guess [,] is another way of writing[][]? the fitting function demands it so I shall deliver
            double[,] position = new double[input.Length,1];
            for (int index = 0; index < input.Length; index++) { position[index,0] = index; }
            //starting parameters [center, sigma, intensity, offset]
            double maxval = input.Max();
            double[] parameters = new double[] { input.ToList().IndexOf(maxval), 10, maxval, input.Average() };
            alglib.lsfitstate state;
            alglib.lsfitreport rep;
            double diffstep = 1e-4;
            //boundary conditions
            //sigma not allowed to go to 0 because it a) unphysical and b) easier to set here than to check later to avoid inf
            double[] bndl = new double[4] {0, 1e-2, 0, 0 };
            double[] bndu = new double[4];
            for (int i = 0; i < 4; i++) { bndu[i] = System.Double.PositiveInfinity; }
            
            //target step size, if step smaller -> converged
            double epsx = 1e-10;
            //maximum iterations
            int maxits = 5000;
            //attempt to get better fit by telling it about variable scale
            double[] scale = new double[] { 100, 1, 1000, 10};


            //create fit

            //conversion of input no longer needed here, done elsewhere
            //double[] tempInput = Array.ConvertAll(input, Convert.ToDouble);
            alglib.lsfitcreatef(position, input, parameters, diffstep, out state);
            alglib.lsfitsetbc(state, bndl, bndu);
            alglib.lsfitsetcond(state, epsx, maxits);
            //makin the fit more stringent again
            //alglib.lsfitsetscale(state, scale);
            alglib.lsfitfit(state, GaussFunction, null, null);
            //the out int info is not documened for this method
            //however from other methods I expect that return value 1 signifies everything went well
            alglib.lsfitresults(state, out int info, out parameters, out rep);
            /*
            switch (info)
            {
                case 2: { Console.WriteLine("Good fit"); }
                    break;
                case 5: { Console.WriteLine("Max its reached"); }
                    break;
                case 7: { Console.WriteLine("Stopping conditions too stringent"); }
                    break;
            }
            */
            if (info > 0) { return parameters; }
            //Console.WriteLine("Bad fit");
            //return parameters;
            return null;
        }


        public static void GaussFunction(double[] c, double[] x, ref double func, object obj)
        {
            ///c[0] is center position
            ///c[1] is sigma
            ///c[2] is intensity
            ///c[3] is baseline offset
            func = System.Math.Exp(-System.Math.Pow((x[0] - c[0] )/ c[1], 2) / 2) * c[2] + c[3];
        }
        public static double GaussFunction(double[] c, double x)
        {
            ///c[0] is center position
            ///c[1] is sigma
            ///c[2] is intensity
            ///c[3] is baseline offset
            return System.Math.Exp(-System.Math.Pow((x - c[0]) / c[1], 2) / 2) * c[2] + c[3];
        }

        //takes c parameters for gaussian and returns f(x) values for vector x
        //normed true if c[2] is sigma, false if it is the prefactor
        private double[] GaussCalc(double[] c, double[] x, bool normed)
        {
            double[] output = new double[x.Length];
            if (c == null) { return output; }

            double[] parameters = new double[4];
            Array.Copy(c, parameters, 4);
            if (normed)
            {
                parameters[2] = 1/(c[1]*Math.Sqrt(Math.PI*2));
                parameters[3] = 0;
            }
            
            
            for (int i = 0; i < x.Length; i++) { output[i] = GaussFunction(parameters, x[i]); }
            return output;
        }

        public short[] RefIntMap 
        {
            get
            {
                return this.m_RefInt;
            }
            set
            {
                this.m_RefInt = value;
                //set the new fit values
                refFit = FitHandler(value);
                //plt.AddHeatmap(shortVectorToMap(m_RefInt, data_width, data_height));
                //plt.AddColorbar();
                this.RefreshImage();
            }
        }

        public short[]  CamIntMap
        {
            get
            {
                return this.m_ImageInt;
            }
            set
            {
                this.m_ImageInt = value;
                //set the new fit values
                cameraFit = FitHandler(value);
                //debugging
                //Console.WriteLine("CamIntMap");

            }
        }

        public int data_height
        {
            get { return this.m_dheight; }
            set { this.m_dheight = value; }
        }
        public int data_width
        {
            get { return this.m_dwidth; }
            set { this.m_dwidth = value; }
        }

        //for correct orientation with AddHeatmap
        public double[,] ShortVectorToMap(short[] inputVector, int x_width, int y_height)
        {
            double[,] Map = new double[y_height, x_width];
            for (int y_index = 0; y_index < y_height; y_index++)
            {
                for (int x_index = 0; x_index < x_width; x_index++)
                {
                    Map[y_index, x_index] = inputVector[x_index + y_index * x_width];
                }
            }
            return Map;
        }
        //overload with another short to subtract background
        //beware x is counting backwards due to the native behavior of m_xStart in the SDK
        //I decided to not invert it because I thought that would be even more confusing for consistency reasons
        public double[,] ShortVectorToMap(short[] inputVector, short[] bgVector, int x_width, int y_height)
        {
            double[,] Map = new double[y_height, x_width];
            for (int y_index = 0; y_index < y_height; y_index++)
            {
                //for (int x_index = 0; x_index < x_width; x_index++)
                for (int x_index = x_width-1; x_index > 0; x_index--)
                {
                    Map[y_index, x_index] = inputVector[x_index + y_index * x_width]
                                            - bgVector[1280 + x_index - m_xStart -x_width + (y_index + m_yStart) * 1280];
                         //+bgVector[(x_index + m_xStart) * x_width + y_index + m_yStart];
                }
            }
            return Map;
        }
        //returns the vector/array  of the position line along the sensor in direction
        //returns normed data (also offset set to 0)
        public double[] PlottableMapSlice(short[] inputVector, axis direction, int position)
        {
            double[] Map;
            double max = 0;
            double min = 4096;
            if (direction == axis.X)
            {
                Map = new double[data_width*sample_rate];
                for (int x_index = 0; x_index < data_width; x_index++)
                {
                    for (int k = 0; k < sample_rate; k++)
                    {
                        Map[x_index + k] = inputVector[x_index + data_width * position];
                    }
                    if (max < Map[x_index]) { max = Map[x_index]; }
                    if (min > Map[x_index]) { min = Map[x_index]; }
                }
            }
            else
            {
                //axis.Y
                Map = new double[data_height*sample_rate];
                for (int y_index = 0; y_index < data_width; y_index++)
                {
                    for (int k = 0; k < sample_rate; k++)
                    {
                        Map[y_index + k] = inputVector[position + data_width * y_index];
                    }

                    if (max < Map[y_index]) { max = Map[y_index]; }
                    if (min > Map[y_index]) { min = Map[y_index]; }
                }
            }
            //for (int i = 0; i < Map.Length; i++) { Map[i] = (Map[i] - min) / (max - min); }
            return Map;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        //creates a title from class variables
        private string TitleMaker()
        {
            //titleTemplate[1][0] = "FWHM : ";
            //titleTemplate[1][1] = "Center : ";
            //titleTemplate[1][2] = "Overlap : ";
            //titleTemplate[1][3] = "Total Overlap : ";
            double[] overlaps = new double[2];
            string output = null;
            foreach (axis axIndex in (axis[]) Enum.GetValues(typeof(axis)))
            {
                //check validity of data
                if ((cameraFit[(int)axIndex] == null)) { continue; }
                overlaps[(int)axIndex] = Overlap(axIndex);
                output += titleTemplate[0][(int)axIndex] + titleTemplate[1][0] + 
                    (Unit(true) * SigToFWHM(cameraFit[(int)axIndex][1])).ToString("00.0") + Unit().ToString() +"\n"
                        + titleTemplate[1][1] + cameraFit[(int)axIndex][0].ToString("000.00") + "\n"
                        + titleTemplate[1][2] + overlaps[(int)axIndex].ToString("0.0000") + "\n";
            }
            //total overlap
            output += titleTemplate[1][3] + (overlaps[0] * overlaps[1]).ToString("0.0000") + "\n";
            //string output = new string(titleTemplate[0][(int)axis.X] + );
            //additionally give the peak radiance
            if (pxConversion != 1) { output += "Peak radiance: " + (peakRadiance * 1e8).ToString("E2") + " W/cm^2"; }
            else { output += "Peak radiance: " + (peakRadiance).ToString("E2") + " W/px^2"; }
            return output;
        }

        //calculate FWHM from sigma
        //maybe overload later for pixel/space input
        public double SigToFWHM(double sig)
        {
            return 2 * Math.Sqrt(2 * Math.Log(2)) * sig;
        }

        //multiply gaussians on top of each other
        //Replaced by the analytic rebase
        //check if fits work, if not return 0
        private double Overlap(axis XorY)
        {
            double analyticOverlapQualifier = 0;
            ///c[0] is center position
            ///c[1] is sigma
            ///c[2] is intensity
            ///c[3] is baseline offset
            if (XorY == axis.X) || (XorY == axis.Y)
            {
                if ((cameraFit[(int)XorY] == null) || (refFit[(int)XorY] == null)) { return 0; }
      
                int sumSigSquare = Math.Pow(refFit[(int)XorY][1],2) + Math.Pow(cameraFit[(int)XorY][1],2);
                analyticOverlapQualifier = Math.Sqrt(2*Math.Pow(refFit[(int)XorY][1]*cameraFit[(int)XorY][1], 2)/sumSigSquare)*Math.Exp(-Math.Pow(cameraFit[(int)XorY][0]-refFit[(int)XorY][0])/(4*sumSigSquare));
            }
            else { throw new Exception("Wrong input into Overlap: needs to be axis.X or axis.Y"); }
            return analyticOverlapQualifier;
            

            /*old simple summation
            double sum = 0;
            if (yIn1.Length != yIn2.Length) { throw new Exception("Overlap: input arrays not same size"); }
            for (int i = 0; i < yIn1.Length; i++) { sum += Math.Sqrt(yIn1[i] * yIn2[i])/sample_rate; }
            //Not sure if I should maybe do this differently
            //sum = Math.Sqrt(2)*sum;
            return sum;
            */
        }

        private unitType Unit()
        {
            if (muToPx == 1) { return unitType.px; }
            else { return unitType.µm; }
        }
        //this is bad but I don't want to think about it
        private double Unit(bool _)
        {
            if (muToPx == 1) { return 1; }
            else { return muToPx; }
        }

        //getters for special settings
        public double pxConversion
        {
            get { return muToPx; } 
            set { muToPx = value; }
        }
        public double PowerLaser
        {
            get { return laserPower; }
            set { laserPower = value; }
        }

        
        public bool plotLiveData 
        { 
            get { return plotLive; }
            set { plotLive = value; }
        }

        //calculate peak radiance of 2D gaussion only on successful fits
        private double peakRadiance
        {
            get
            {
                if (cameraFit[(int)axis.Y] == null || cameraFit[(int)axis.X] == null) { return 0; }
                double peak = PowerLaser / (Math.PI * 2 * cameraFit[(int)axis.X][1] * cameraFit[(int)axis.Y][1]);
                peak = peak / Math.Pow(pxConversion,2);
                return peak;
            }
        }

        //see Reference loading in form1
        public void FetchBackground()
        {
            //return;
            //open subkey
            RegistryKey regkey;
            regkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\LiveFit", false);
            var filepath_key = regkey.GetValue("BACKGROUND_FILE", null);

            //check if value is valid and break if that is the case
            if (filepath_key == null) { return; }
            string filepath = filepath_key.ToString(); 
            //if (filepath.IsNullOrEmpty() == true) { return; }
            if (String.IsNullOrWhiteSpace(filepath)) { return; }
            byte[] inputByteArray = File.ReadAllBytes(filepath);
            //int length_ByteArray = inputByteArray.Length;

            for (int i = 0; i < 1280*720; i++)
            {
                m_BackgroundRemoveInt[i] = (short)(((((Int16)inputByteArray[(3 * i) * 2 + 1]) & 0b1111) << 8) | (inputByteArray[(3 * i) * 2]));
            }
        
        }
        //accepts the starting positions for the current camera frame used
        //sadly no cross checking yet, need to think about that if I have time
        public int[] cameraFrame
        {
            set
            {
                m_xStart = value[(int)axis.X];
                m_yStart = value[(int)axis.Y];
            }
            get
            {
                int[] out_Pos = { m_xStart, m_yStart};
                return out_Pos;
            }
        }

        private void TestLiveImage()
        {
            var plt = new ScottPlot.Plot(600, 400);
            double[,] tempMap = ShortVectorToMap(m_ImageInt, m_BackgroundRemoveInt, m_dwidth, m_dheight);
            plt.AddHeatmap(tempMap);
            //plt.AddHeatmap(ShortVectorToMap(m_ImageInt, m_dwidth, m_dheight));
            //plt.AddHeatmap(ShortVectorToMap(m_BackgroundRemoveInt, m_dwidth, m_dheight));
            ScottPlot.Plottable.Colorbar cb = plt.AddColorbar();
            cb.MinValue = 0;
            cb.MaxValue = (from double v in tempMap select v).Max();

            new ScottPlot.FormsPlotViewer(plt).ShowDialog();

        }
    }
}
