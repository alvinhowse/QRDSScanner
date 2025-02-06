using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Aztec;
using System.Threading;
using System.Diagnostics;
using System.Net;

class Program
{
	static Bitmap bitmapper;
    static bool photoTaken = false; // A flag to indicate if a photo has been taken
	static bool abletochooseagain = true;

    static void Main()
    {
        // Create a filter for available video devices (webcams)
        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

        // Check if any webcams are found
        if (videoDevices.Count == 0)
        {
            Console.WriteLine("No webcam detected.");
            return;
        }

        // Use the first available webcam
		if (abletochooseagain) {
		var captureornot = MessageBox.Show("Would you like to capture the QR code? If not, the program can use the most recent capture of it", "qrds view", MessageBoxButtons.YesNo);
		if (captureornot == DialogResult.Yes) {
        VideoCaptureDevice webcam = new VideoCaptureDevice(videoDevices[0].MonikerString);

        // Attach an event handler to process the video frame
        webcam.NewFrame += new NewFrameEventHandler((sender, eventArgs) => Webcam_NewFrame(sender, eventArgs, webcam));

        // Start capturing video
        webcam.Start();
        Thread.Sleep(1000);
		}
        Runner();
		}
		else {
			VideoCaptureDevice webcam = new VideoCaptureDevice(videoDevices[0].MonikerString);

        // Attach an event handler to process the video frame
        webcam.NewFrame += new NewFrameEventHandler((sender, eventArgs) => Webcam_NewFrame(sender, eventArgs, webcam));

        // Start capturing video
        webcam.Start();
        Thread.Sleep(1000);
		
        Runner();
		}
    }

    static void Webcam_NewFrame(object sender, NewFrameEventArgs eventArgs, VideoCaptureDevice webcam)
    {
        // Check if we've already taken a photo
        if (!photoTaken)
        {
            // Get the current video frame
            using (var frame = (System.Drawing.Bitmap)eventArgs.Frame.Clone())
            {
                string fileName = "disk.jpg";
                frame.Save(fileName);
				bitmapper = frame;
            }

            // Set the flag to true and stop the webcam
            photoTaken = true;
            webcam.SignalToStop();
        }
	}
		static void Runner()
        {
			try {
		BarcodeReader Reader = new BarcodeReader();
                Result result = Reader.Decode(new Bitmap("disk.jpg"));
                try
                {
                    string decoded = result.ToString();
                    if (decoded != "")
                    {
                        File.WriteAllBytes("qrmount.vhdx", File.ReadAllBytes("bup.vhdx"));
						Process.Start("qrmount.vhdx");
						Thread.Sleep(1000);
						var filename = "";
						var linesofqr = decoded.Split('\n');
								File.WriteAllText("encoded.txt", decoded);
								Process.Start("certutil", "-decode encoded.txt decoded.zip");
								Thread.Sleep(1000);
								Process.Start("C:\\Program Files\\7-Zip\\7z.exe", "x decoded.zip -oZ:\\");
                    }
                }
                catch(Exception ex){
					if (ex.Message == "Object reference not set to an instance of an object.") {
						abletochooseagain = false;
						Main();
					}
					else {
MessageBox.Show("qrds.runner.error (" + ex.Message + ")", "qrds view (2)");
					}

                }
		}
				catch(Exception ex){
					MessageBox.Show("qrds.runner.error (" + ex.Message + ")", "qrds view");
                }
		}
    }
