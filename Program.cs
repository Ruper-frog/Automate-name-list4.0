using Automate_name_list4_0;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.ML.Data;
using OpenPop.Mime;
using OpenPop.Pop3;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Automate_name_list4._0
{
    internal class Program
    {
        // TODO: change to Size variable Bype.

        static int CMD_Height = Console.WindowHeight, CMD_width = Console.WindowWidth;

        static string HomeDirectoryOfTheProgram = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\.."));
        static string InProcessDirectory = Path.Combine("In process");
        static string OutProcessDirectory = Path.Combine(HomeDirectoryOfTheProgram, "Email records");

        static string Username = "namelistp@gmail.com", Password = "nysl jwxw gddw xayt";
        static string EmailSender, EmailSubject;

        static int EmailsCount;
        static bool EmailContainsImage = false;

        static string ImageLocation;
        //static string ImageLocation = @"C:\Users\USER\OneDrive\שולחן העבודה\Name list images\OriginalImages\11.jpg";

        static string FolderLocation, OutputImageLocation;

        static bool[,] PhoneNetwork;
        static int NumPerPocketColumn, NumPerPocketRow;

        static Point[] Corners = new Point[4];
        static Point[] CroppedCorners = new Point[4];

        static (double Width, double Height) PocketTable = (65.9892, 93.4974);
        static (double Width, double Height) SinglePocket = (10.0076, 12.0142);
        static double ImageToRealRatio;

        static List<bool> Line = new List<bool>();
        static int LineSuccessRates = 70;

        static int ColorDistLimit = 1;
        //static Color CornerColor = Color.FromArgb(60, 50, 51);
        //static Color NumberColor = Color.FromArgb(255, 255, 255);
        //static Color YellowBackgroundPocketColor = Color.FromArgb(255, 254, 76);


        static Color CornerColor = Color.FromArgb(60, 50, 51);
        static Color NumberColor = Color.FromArgb(254, 254, 254);
        static Color YellowBackgroundPocketColor = Color.FromArgb(255, 215, 0);


        static string[] ImageAllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }; // Add more image extensions as needed
        static string[] TextAllowedExtensions = { ".txt", ".text" }; // Add more text extensions as needed

        static string nameListLocation;
        static string[] FileLines;

        static List<int> ClassIndexes = new List<int>();
        static int ClassNum;
        static List<string> Names = new List<string>();
        static void Main(string[] args)
        {
            //TODO: Perhaps make like a sliding announcement when an Email was received.

            //TODO: get ride of methods with ref init(British accent).

            //TODO: return an email if the program couldn't work on the image.

            //TODO: add some colors to it.

            //System.Environment.Exit(0);

            // Leptop
            //ImageLocation = ImageLocation.Replace("USER", "ruper");
            //ImageEnhancedLocation = ImageEnhancedLocation.Replace("USER", "ruper");
            //ImageCroppedLocation = ImageCroppedLocation.Replace("USER", "ruper");
            //ImageObjectsCornersLocation = ImageObjectsCornersLocation.Replace("USER", "ruper");
            //ImagePerspectiveTransformationLocation = ImagePerspectiveTransformationLocation.Replace("USER", "ruper");
            //ImageGridLocation = ImageGridLocation.Replace("USER", "ruper");


            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.UTF8; // Reset to the default encoding (UTF-8)
            //Console.OutputEncoding = Encoding.GetEncoding("Windows-1255");//Changing the console window to a format that will be able to present Hebrew text.


            // Clean: Clean this mess.
            //for (int T = 1; T <= 18; T++)
            //{
            //    ImageLocation = Path.GetDirectoryName(ImageLocation) + "/" + T + ".jpg";
            //    ProcessImage();
            //}
            //System.Environment.Exit(0);

            while (true)
            {
                ValidatingTheHomeHomeDirectoryOfTheProgram();
                FileLines = File.ReadAllLines(nameListLocation);//Extracting the information in the name list file.
                FileLines = Array.ConvertAll(FileLines, s => s.Trim());

                //Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Total messages: " + EmailsCount);

                Console.SetCursorPosition(0, 1);
                //Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("TO LEAVE PRESS THE ESCAPE KEY");
                Console.ResetColor();

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine();
                        break; // Exit the loop when the Escape key is pressed
                    }
                }

                if (!Directory.Exists(InProcessDirectory))
                    Directory.CreateDirectory(InProcessDirectory);

                if (!Directory.Exists(OutProcessDirectory))
                    Directory.CreateDirectory(OutProcessDirectory);

                FetchTheImageFromEmail();

                if (EmailsCount == 0)
                    continue;
                else if (!EmailContainsImage)
                    continue;

                List<string> namesTemp = new List<string>();//Making a temporary list that would contains the names of each class each time and would sort it.
                int LineIndex;//A variable that indexes which line we are going through.
                for (int i = 0; i < ClassIndexes.Count(); i++)//Sorting the name of each class by the alphabet.
                {
                    for (int j = 0; ClassIndexes[i] + 1 + j < FileLines.Count(); j++)//A loop that would run as many times as the number of names in each class.
                    {
                        LineIndex = ClassIndexes[i] + 1 + j;//Getting the selectedIndex of the line.
                        if (FileLines[LineIndex].Equals(""))//If the line is empty we reached the end of the names in the current class.
                            break;
                        namesTemp.Add(FileLines[LineIndex]);//We add the names of the students to the temporary name list.
                    }
                    //namesTemp = namesTemp.Select(s => s.Trim()).ToList();//Getting rid off unwanted spaces in the string that contains the names of the students.
                    namesTemp.Sort();//Sorting the names by the Hebrew Alphabet.
                    for (int j = 0; j < namesTemp.Count(); ++j)//A loop that would run as many times as the number of names in the list.
                    {
                        LineIndex = ClassIndexes[i] + 1 + j;//Getting the selectedIndex of the line.
                        if (LineIndex.Equals(""))//If the line is empty we reached the end of the names in the current class.
                            break;
                        FileLines[LineIndex] = namesTemp[j];//Rewriting the names by order to the array that contains the content of the name list text file
                    }
                    namesTemp.Clear();//Clearing the names from the temporary name list and getting it ready for the next class.
                }
                File.WriteAllLines(nameListLocation, FileLines);//Rewriting all the names to the original name list text file.

                Names.Clear();
                for (int i = ClassIndexes[ClassNum] + 1; i != FileLines.Length && (i != ClassIndexes.Count - 1 || i != ClassIndexes[ClassNum + 1]); i++)//A loop that saves the names of the chosen class.
                {
                    if (FileLines[i].Equals(""))//If the line is empty we reached the end of the names in the current class.
                        break;

                    string name = FileLines[i];//Extracting the name of the file text array.
                                               //MakeSpaces(ref name);
                    Names.Add(name);//Adding the name to the name list that contains the names of the students of the chosen class.
                }

                bool[] attendance = CutArrayPerClass(ProcessImage());//Getting the array of the board and transforming it to the names of the students and making sure present by the amount of students in the class.

                //string[] replyNamesTrue = new string[attendance[0].Length];
                //for (int y = 0; y < attendance[0].Length; y++)
                //    replyNamesTrue[y] = Names[attendance[0][y]];

                //string[] replyNamesFalse = new string[attendance[1].Length];
                //for (int y = 0; y < attendance[1].Length; y++)
                //    replyNamesFalse[y] = Names[attendance[1][y]];

                string replyBody;
                char checkmark = '\u2713'; // Checkmark symbol (√)
                char crossMark = '\u2717'; // Cross mark symbol (✗)

                if (Names.Count() > 0)
                {
                    // Add CSS styles for borders and padding
                    replyBody = "<table style='direction: rtl; border: 1px solid black; border-collapse: collapse;'><tr><th style='border: 1px solid black; padding: 8px;'> </th><th style='border: 1px solid black; padding: 8px;'>תלמיד/ה</th><th style='border: 1px solid black; padding: 8px;'>חסר</th><th style='border: 1px solid black; padding: 8px;'>נוכח</th></tr>";

                    for (int i = 0; i < Names.Count(); i++)
                    {
                        // Apply styles to individual cells
                        string cellStyle = "border: 1px solid black; padding: 8px;";
                        replyBody += "<tr><td style='" + cellStyle + "'>" + (i + 1) + "</td><td style='" + cellStyle + "'>" + Names[i] + "</td><td style='" + cellStyle + "'>";

                        if (attendance[i])
                            replyBody += crossMark.ToString() + "</td><td style='" + cellStyle + "'>" + " " + "</td></tr>";
                        else
                            replyBody += " " + "</td><td style='" + cellStyle + "'>" + checkmark.ToString() + "</td></tr>";
                    }
                    replyBody += "</table>";
                }
                else
                    replyBody = "No students were found";


                // Send the reply email
                SendEmail(EmailSender, EmailSubject, replyBody);
                Console.Clear();
            }
        }
        static void ValidatingTheHomeHomeDirectoryOfTheProgram()
        {
            while (true)//Looping until the path that the user provided  has checks out.
            {
                try
                {
                    if (ValidateImagePath(HomeDirectoryOfTheProgram))
                        break;
                }
                catch (Exception ex)//Handling the problems in the path that the user provided.
                {
                    Console.Clear();
                    Console.WriteLine("Error: " + ex.Message);//Printing the problem message.
                    Console.WriteLine("Please drag the name list file into the folder of the program, once the file is inside press any button");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            bool ValidateImagePath(string FolderPath)
            {
                nameListLocation = Directory.GetFiles(FolderPath)
                    .FirstOrDefault(file => TextAllowedExtensions.Contains(Path.GetExtension(file).ToLower()));

                if (string.IsNullOrEmpty(nameListLocation))
                    throw new Exception("The folder does not contains any name list.");
                return true;
            }
        }//Perhaps: add some way to delete emails so it won't fill up.
        static void CollectingThePathFromTheFrontDesk()
        {
            string insideFolderPath = "User preferences saved";//The name of the folder that contains the file that contains the user preferences.
            string insideFolderTextFilePath = Path.Combine(insideFolderPath, "User preferences.txt");//The name of the file that contains the user preferences.

            if (!Directory.Exists(insideFolderPath))//Checking to see if the folder already exists.
                Directory.CreateDirectory(insideFolderPath);//Creating if it doesn't.

            while (true)//Looping until the path that the user provided  has checks out.
            {
                try
                {
                    if (!File.Exists(insideFolderTextFilePath))//Checking to see if the file already exists.
                    {
                        if (string.IsNullOrEmpty(FolderLocation))
                        {
                            Console.CursorVisible = true;
                            Console.Clear();

                            Console.WriteLine("Please enter the folder location or simply drag the file in here");
                            string userInput = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(userInput))
                            {
                                Console.WriteLine("Invalid input. Please reenter the folder location or drag an image in here.");
                                continue; // Skip the rest of the loop and ask for input again.
                            }
                            if (userInput[0].Equals('\"'))//If the user dragged the file it will have quotation rapping it.
                                userInput = userInput.Trim('\"');//removing those quotation.
                            FolderLocation = Path.GetDirectoryName(userInput);//If it doesn't ask the user to provide a path to the imagesDates.
                            Console.Clear();
                        }

                        if (ValidateImagePath(FolderLocation))//Checking if the path checks out and assigning the locations to the variables.
                        {
                            File.WriteAllText(insideFolderTextFilePath, FolderLocation);//Saving the path that the user provided to a text file.
                            break;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(FolderLocation))
                        {
                            string[] location = File.ReadAllLines(insideFolderTextFilePath);//If the file exist extract the path from it
                            FolderLocation = location[0];//Extracting the path from the file.
                        }

                        if (ValidateImagePath(FolderLocation))//Checking if even the saved file is valid because the user could have change the location of the imagesDates from the last run.
                        {
                            File.WriteAllText(insideFolderTextFilePath, FolderLocation);//Saving the path that the user provided to a text file.
                            break;
                        }
                    }
                }
                catch (Exception ex)//Handling the problems in the path that the user provided.
                {
                    Console.Clear();
                    Console.WriteLine("Error: " + ex.Message);//Printing the problem message.
                    Console.WriteLine("The folder was not valid, Please enter the folder location or simply drag the file in here");

                    Console.CursorVisible = true;

                    string userInput = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(userInput))
                    {
                        Console.WriteLine("Invalid input. Please reenter the folder location or drag an image in here.");
                        continue; // Skip the rest of the loop and ask for input again.
                    }
                    if (userInput[0].Equals('\"'))//If the user dragged the file it will have quotation rapping it.
                        userInput = userInput.Trim('\"');//removing those quotation.
                    FolderLocation = Path.GetDirectoryName(userInput);//If there were a problem with the path aske the user to enter a new one.
                    Console.Clear();
                }
            }
            bool ValidateImagePath(string FolderPath)
            {
                nameListLocation = Directory.GetFiles(FolderPath)
                    .FirstOrDefault(file => TextAllowedExtensions.Contains(Path.GetExtension(file).ToLower()));

                if (string.IsNullOrEmpty(nameListLocation))
                    throw new Exception("The folder does not contains any name list.");
                return true;
            }
            Console.CursorVisible = false;
        }//no need of the function because we are using the HomeDirectoryOfTheProgram varaibe instead.

        static void FetchTheImageFromEmail()
        {
            //TODO: Consider adding some nice sliding animation when an email is received.

            string pop3Server = "pop.gmail.com";
            int pop3Port = 995; // Port for SSL-enabled POP3
            bool useSsl = true;
            Username = "namelistp@gmail.com";
            Password = "nysl jwxw gddw xayt";

            using (Pop3Client pop3Client = new Pop3Client())
            {
                pop3Client.Connect(pop3Server, pop3Port, useSsl);
                pop3Client.Authenticate(Username, Password);

                EmailsCount = pop3Client.GetMessageCount();
                //Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Total messages: " + EmailsCount);
                Console.ResetColor();

                if (EmailsCount > 0)
                {
                    Console.Clear();
                    //Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Total messages: " + EmailsCount);

                    //Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Message message = pop3Client.GetMessage(1);
                    Console.WriteLine("Subject: " + message.Headers.Subject);
                    Console.WriteLine("From: " + message.Headers.From.Address);

                    // Extract information from the original email
                    EmailSender = message.Headers.From.Address;
                    EmailSubject = message.Headers.Subject;
                    ClassNum = WhichClass();//In the case of multiple classes the user needs to choose in which class the image was taken.

                    EmailSubject = "Re: " + EmailSubject; // Modify the EmailSubject as needed

                    // Compose your reply email
                    //string replyBody = "Thank you for using our lovely services";

                    // Iterate through message's attachments
                    foreach (OpenPop.Mime.MessagePart attachment in message.FindAllAttachments())
                    {
                        // Check if the attachment is an image
                        string fileName = attachment.FileName; // Get the file name of the image
                        byte[] imageBytes = attachment.Body;  // Get the image data as bytes

                        // Check if the attachment is an image based on its file extension
                        string fileExtension = Path.GetExtension(fileName).ToLower();

                        if (ImageAllowedExtensions.Contains(fileExtension))
                        {
                            EmailContainsImage = true;

                            // Empty the folder before saveing the image
                            // Get the list of files in the folder and delete them
                            Directory.EnumerateFiles(InProcessDirectory).ToList().ForEach(File.Delete);// TODO: change this cuz if theres a lot of trafic this will erase all the other messages

                            // Save the image to a file
                            string fileLocation = Path.Combine(InProcessDirectory, fileName);
                            File.WriteAllBytes(fileLocation, imageBytes);

                            Image image = Image.FromFile(fileLocation);
                            RotateImageIfNecessaryForMatForImage(ref image, fileLocation);
                            Bitmap imageBitmap = new Bitmap(image);
                            image.Dispose();
                            imageBitmap.Save(fileLocation);
                            imageBitmap.Dispose();

                            Console.WriteLine("Image saved: " + fileName);
                        }
                    }
                    Console.WriteLine("----------");

                    pop3Client.Disconnect();
                    if (!EmailContainsImage)
                    {
                        SendEmail(EmailSender, EmailSubject, "סליחה אבל האימייל ששלחת לא מכיל שום תמונה." + "\nSorry but the email that you have send does not contain any images.");
                        Console.Clear();
                    }
                }
            }
        }
        static void SendEmail(string to, string subject, string body/*, string attachmentFilePath*/)
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            bool enableSsl = true;

            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = new NetworkCredential(Username, Password);

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(Username);
                    mailMessage.To.Add(to);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    // Attach the saved image to the reply email
                    //mailMessage.Attachments.Add(new Attachment(attachmentFilePath));

                    smtpClient.Send(mailMessage);
                }
            }
        }

        static int WhichClass()
        {
            string[] options = FindClasses();
            EmailSubject = EmailSubject.Trim();
            int selectedIndex = Array.IndexOf(options, EmailSubject);

            return selectedIndex;

            string[] FindClasses()
            {
                List<string> classes = new List<string>();

                ClassIndexes.Add(0);
                classes.Add(FileLines[0]);// just takes the last char and add it at front with space

                for (int i = 1; i < FileLines.Length - 1; i++)
                {
                    if (FileLines[i].Equals("") && FileLines[i - 1].Equals("") && i != FileLines.Length)
                    {
                        ClassIndexes.Add(i + 1);
                        classes.Add(FileLines[i + 1]);// just takes the last char and add it at front with space
                    }
                }
                return classes.ToArray();
            }
        }

        static bool[] ProcessImage()
        {
            // TODO: Consider switching to the HSL type of color detection in some of the function or just try to integrate it a little more.

            // TODO: Both blue and Yellow.

            // TODO: Litarate through the rectangles and find the right one.

            // TODO: Find the balance in the accuracy.

            if (string.IsNullOrEmpty(ImageLocation))
                ImageLocation = GetImageAndBuildPalace();

            OutputImageLocation = Path.Combine(Path.GetDirectoryName(ImageLocation), Path.GetFileNameWithoutExtension(ImageLocation)) + ".2.0" + ".jpg";
            //OutputImageLocation = Path.Combine(@"C:\Users\USER\OneDrive\שולחן העבודה\Name list images\OriginalImages\New folder", Path.GetFileName(ImageLocation));

            Image image = Image.FromFile(ImageLocation);
            RotateImageIfNecessaryForMatForImage(ref image, ImageLocation);
            Bitmap imageBitmap = new Bitmap(image);
            image.Dispose();
            imageBitmap.Save(ImageLocation);
            imageBitmap.Dispose();

            Bitmap objectsBigArea = new Bitmap(MLUsage());
            //objectsBigArea.Save(OutputImageLocation);
            //System.Environment.Exit(0);

            int blurIntensity = 5; // Adjust this value to control the intensity of the blur

            Mat img = new Mat();
            BitmapToMat(objectsBigArea, img);
            objectsBigArea.Dispose();
            //img.Save(OutputImageLocation);
            //System.Environment.Exit(0);

            // Convert the image to grayscale
            Mat grayImage = new Mat();
            CvInvoke.CvtColor(img, grayImage, ColorConversion.Bgr2Gray);
            //img.Dispose();

            // Enhance image before finding rectangle for better accuracy.
            Mat enhancedImage = EnhanceImage(grayImage, 1);

            // Apply Gaussian Blur with adjustable intensity
            CvInvoke.GaussianBlur(enhancedImage, grayImage, new Size(blurIntensity, blurIntensity), 0);
            enhancedImage.Dispose();
            //CvInvoke.Imwrite(OutputImageLocation, grayImage);

            // Apply Canny edge detection
            Mat edges = new Mat();
            CvInvoke.Canny(grayImage, edges, 100, 200);
            //CvInvoke.Imwrite(OutputImageLocation, edges);
            grayImage.Dispose();


            // Find contours in the edge-detected image
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(edges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            edges.Dispose();

            // Create a list to store the contours along with their corresponding rectangle areas
            List<Tuple<VectorOfPoint, double>> contourAreaList = new List<Tuple<VectorOfPoint, double>>();

            // Iterate through the contours to calculate rectangle areas and store them in the list
            for (int i = 0; i < contours.Size; i++)
            {
                RotatedRect minAreaRect = CvInvoke.MinAreaRect(contours[i]);
                Rectangle rect = minAreaRect.MinAreaRect();

                double rectArea = rect.Width * rect.Height;

                contourAreaList.Add(new Tuple<VectorOfPoint, double>(contours[i], rectArea));
            }

            // Sort the list in descending order based on rectangle areas
            contourAreaList = contourAreaList.OrderByDescending(t => t.Item2).ToList();

            /*// Iterate through the sorted list to process contours by area
            foreach (var contourArea in contourAreaList)
            {
                VectorOfPoint contour = contourArea.Item1;

                RotatedRect minAreaRect = CvInvoke.MinAreaRect(contour);
                // Draw the largest rectangle on the original image
                CvInvoke.Rectangle(img, minAreaRect.MinAreaRect(), new MCvScalar(0, 0, 255), 2);
            }*/

            PointF[] cornersPoints = new PointF[4];

            // Use a for loop to process contours by area
            for (int i = 0; i < 1; i++)
            {
                VectorOfPoint contour = contourAreaList[i].Item1;

                RotatedRect minAreaRect = CvInvoke.MinAreaRect(contour);

                // Get the coordinates of the four cornersPoints
                cornersPoints = minAreaRect.GetVertices();

                // Convert the PointF coordinates to Point (rounding or casting as needed)
                Corners = Array.ConvertAll(cornersPoints, Point.Round);

                // Draw the largest rectangle on the original image
                //CvInvoke.Rectangle(img, minAreaRect.MinAreaRect(), new MCvScalar(0, 0, 255), 2);

                // Now 'corners' contains the four corner points of the rectangle
                // You can use 'corners' array as needed
            }


            // Draw the largest rectangle on the original image
            //CvInvoke.Rectangle(img, largestRotatedRect.MinAreaRect(), new MCvScalar(0, 0, 255), 2);
            //CvInvoke.Imwrite(OutputImageLocation, img);
            //System.Environment.Exit(0);
            //img.Dispose();
            //continue;



            // Extracting the coordinantes from the string.
            //Corners = Array.ConvertAll(cornersPoints, pf => new Point((int)Math.Round(pf.X), (int)Math.Round(pf.Y)));

            // Orgenizing the Y Coordinates.
            SelectionSortBy_Y(Corners);

            Point[] Top = { Corners[0], Corners[1] };

            // Orgenizing the X Coordinates.
            SelectionSortBy_X(Top);

            Point[] Bottom = { Corners[2], Corners[3] };
            SelectionSortBy_X(Bottom);

            // Revaluating the array by the right corners locations.
            Corners = Top.Concat(Bottom).ToArray();

            AlignRectangleCorners();


            //image = Image.FromFile(ImageLocation);
            //RotateImageIfNecessaryForMatForImage(ref image, ImageLocation);

            //Bitmap picture = new Bitmap(img.ToImage<Bgr, Byte>().ToBitmap());
            //img.Dispose();
            ////picture.Save(OutputImageLocation);
            ////System.Environment.Exit(0);
            //image.Dispose();

            Mat croppedImage = CropImage(img, Corners);
            img.Dispose();



            //croppedImage.Save(OutputImageLocation);
            //System.Environment.Exit(0);


            Corners[0] = new Point(0, 0);
            Corners[1] = new Point(croppedImage.Width - 1, 0);
            Corners[2] = new Point(0, croppedImage.Height - 1);
            Corners[3] = new Point(croppedImage.Width - 1, croppedImage.Height - 1);

            //// Orgenizing the Y Coordinates.
            //SelectionSortBy_Y(Corners);

            //Point[] Top1 = { Corners[0], Corners[1] };

            //// Orgenizing the X Coordinates.
            //SelectionSortBy_X(Top1);

            //Point[] Bottom1 = { Corners[2], Corners[3] };
            //SelectionSortBy_X(Bottom1);

            //// Revaluating the array by the right corners locations.
            //Corners = Top1.Concat(Bottom1).ToArray();

            //AlignRectangleCorners();


            FindObjectsCorners(croppedImage);
            //croppedImage.Save(OutputImageLocation);
            //System.Environment.Exit(0);
            //continue;

            Mat transformedImage = PerformPerspectiveTransformation(croppedImage, Corners);
            croppedImage.Dispose();
            CvInvoke.Imwrite(OutputImageLocation, transformedImage);
            //System.Environment.Exit(0);
            //continue;





            //Bitmap transformedImageBitmap = new Bitmap(transformedImage.ToImage<Bgr, Byte>().ToBitmap());




            Bitmap thresholded = BlackItOut(transformedImage);



            ImageToRealRatio = thresholded.Width / PocketTable.Width;
            (int Width, int Height) SinglePocketImageSize = ((int)(ImageToRealRatio * SinglePocket.Width), (int)(ImageToRealRatio * SinglePocket.Height));

            CheckIfThePcketsAreBlueOrYello(thresholded, ref transformedImage, SinglePocketImageSize);
            //transformedImage.Save(OutputImageLocation);
            //return null;

            thresholded = BlackItOut(transformedImage);
            //thresholded.Save(OutputImageLocation);
            //return null;


            transformedImage.Dispose();
            //thresholded.Save(OutputImageLocation);
            //continue;

            Bitmap thresholdedWithBlack = new Bitmap(thresholded);

            for (int x = thresholdedWithBlack.Width - 1; x >= 0; x--)
            {
                for (int y = thresholdedWithBlack.Height - 1; y >= 0; y--)
                {
                    if (!ColorDist(thresholdedWithBlack.GetPixel(x, y), Color.Black))
                        thresholdedWithBlack.SetPixel(x, y, Color.Black);
                    else break;
                }
            }

            for (int y = 0; y < thresholdedWithBlack.Height - 1; y++)
            {
                for (int x = 0; x < thresholdedWithBlack.Width - 1; x++)
                {
                    if (!ColorDist(thresholdedWithBlack.GetPixel(x, y), Color.Black))
                        thresholdedWithBlack.SetPixel(x, y, Color.Black);
                    else break;
                }
            }

            for (int y = 0; y < thresholdedWithBlack.Height - 1; y++)
            {
                for (int x = thresholdedWithBlack.Width - 1; x >= 0; x--)
                {
                    if (!ColorDist(thresholdedWithBlack.GetPixel(x, y), Color.Black))
                        thresholdedWithBlack.SetPixel(x, y, Color.Black);
                    else break;
                }
            }

            //transformedImage.Dispose();
            //imageWithGrids.Save(OutputImageLocation);
            //continue;





            NumPerPocketColumn = thresholdedWithBlack.Width / SinglePocketImageSize.Width;
            NumPerPocketRow = CountNumber(ref thresholdedWithBlack, SinglePocketImageSize);
            //Console.WriteLine(T + ": " + NumPerPocketRow);
            //imageWithGrids.Save(OutputImageLocation);
            thresholdedWithBlack.Dispose();
            //continue;




            PhoneNetwork = CountPhones(ref thresholded, SinglePocketImageSize);// TODO: before runing the function make sure to saparate the BlackItOut function from the black part and the cropped part.
            //thresholded.Save(OutputImageLocation);
            //return null;
            thresholded.Dispose();

            //Console.Clear();
            //System.Threading.Thread.Sleep(100);

            //for (int y = 0; y < PhoneNetwork.GetLength(0); y++)
            //{
            //    for (int j = 0; j < PhoneNetwork.GetLength(1); j++)
            //        Console.Write(PhoneNetwork[y, j] ? "[1]" : "[0]");
            //    Console.WriteLine();
            //}
            //System.Threading.Thread.Sleep(3000);


            bool[] phoneNetworkFlat = new bool[PhoneNetwork.GetLength(0) * PhoneNetwork.GetLength(1)];
            for (int row = 0, index = 0; row < PhoneNetwork.GetLength(0); row++)
            {
                for (int col = 0; col < PhoneNetwork.GetLength(1); col++)
                {
                    phoneNetworkFlat[index] = PhoneNetwork[row, col];
                    index++;
                }
            }

            //int[] trueIndexes = Enumerable.Range(0, phoneNetworkFlat.Length)
            //.Where(y => phoneNetworkFlat[y] == true).ToArray();

            //int[] falseIndexes = Enumerable.Range(0, phoneNetworkFlat.Length)
            //.Where(y => phoneNetworkFlat[y] == false).ToArray();

            //int[][] attendance = { trueIndexes, falseIndexes };
            //return attendance;

            return phoneNetworkFlat;

            /*int jumps = imageWithGrids.Width / 12;
            //for (int y = jumps; y < imageWithGrids.Width; y += jumps * 2)
            //{
            //    for (int j = 0; j < imageWithGrids.Height; j++)
            //    {
            //        imageWithGrids.SetPixel(y, j, Color.Red);
            //    }
            //}*/


            //Console.WriteLine("Image cropped and saved successfully.");

            //Process.Start(OutputImageLocation);
            //Process.Start(PerspectiveTransformationPath);
            //Process.Start(PerspectiveTransformationWithGridPath);
        }
        static Bitmap MLUsage()
        {
            // Create single instance of sample data from the first line of the dataset for model input.
            var image = MLImage.CreateFromFile(ImageLocation);
            MLModel.ModelInput sampleData = new MLModel.ModelInput()
            {
                Image = image,
            };

            // Make a single prediction on the sample data and print results.
            var predictionResult = MLModel.Predict(sampleData);

            //Console.WriteLine("\n\nPredicted Boxes:\n");

            if (predictionResult.PredictedBoundingBoxes == null)
            {
                Console.WriteLine("No Predicted Bounding Boxes");
                return null;
            }

            //// Rest of your code to print bounding box coordinates and scores
            //var boxes = predictionResult.PredictedBoundingBoxes.Chunk(4)
            //    .Select(x => new { XTop = x[0], YTop = x[1], XBottom = x[2], YBottom = x[3] })
            //    .Zip(predictionResult.Score, (a, b) => new { Box = a, Score = b });

            //foreach (var item in boxes)
            //{
            //    Console.WriteLine($"XTop: {item.Box.XTop},YTop: {item.Box.YTop},XBottom: {item.Box.XBottom},YBottom: {item.Box.YBottom}, Score: {item.Score}");
            //}


            float[] boundingBoxes = predictionResult.PredictedBoundingBoxes, scores = predictionResult.Score;

            Bitmap bitmap = new Bitmap(ImageLocation);



            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Find the index of the bounding box with the highest accuracy
                int maxIndex = 0;
                float maxScore = 0;

                for (int i = 0; i < scores.Length; i++)
                {
                    if (scores[i] > maxScore)
                    {
                        maxScore = scores[i];
                        maxIndex = i;
                    }
                }

                // Draw only the rectangle with the highest accuracy
                float xTop = boundingBoxes[maxIndex];
                float yTop = boundingBoxes[maxIndex + 1];
                float xBottom = boundingBoxes[maxIndex + 2];
                float yBottom = boundingBoxes[maxIndex + 3];

                float width = xBottom - xTop;
                float height = yBottom - yTop;

                xTop -= xTop - width / 8 < 0 ? xTop : width / 8;
                yTop -= yTop - height / 8 < 0 ? yTop : height / 8;

                width += width / 4;
                height += height / 4;

                width -= xTop + width >= bitmap.Width ? xTop + width - bitmap.Width : 0;
                height -= yTop + height >= bitmap.Height ? yTop + height - bitmap.Height : 0;

                Rectangle rect = new Rectangle((int)xTop, (int)yTop, (int)width, (int)height);

                // Draw rectangle
                //g.DrawRectangle(Pens.Red, rect);

                // Draw accuracy text above the rectangle
                float score = scores[maxIndex];
                string accuracyText = $"Accuracy: {score:P2}";
                Font font = new Font("Arial", 10);
                //g.DrawString(accuracyText, font, Brushes.Red, xTop, yTop - 15);

                // Crop a new image from the rectangle's corners using CropImage function
                PointF[] corners = new PointF[]
                {
                    new PointF(rect.Left, rect.Top),
                    new PointF(rect.Right, rect.Top),
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.Left, rect.Bottom)
                };

                Bitmap croppedImage = CropImage(bitmap, corners);
                return croppedImage;
            }
        }
        private static Bitmap CropImage(Bitmap originalImage, PointF[] corners)
        {
            int minX = (int)corners.Min(p => p.X);
            int minY = (int)corners.Min(p => p.Y);
            int maxX = (int)corners.Max(p => p.X);
            int maxY = (int)corners.Max(p => p.Y);

            int width = maxX - minX;
            int height = maxY - minY;

            Rectangle cropRect = new Rectangle(minX, minY, width, height);

            Bitmap croppedImage = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(originalImage, 0, 0, cropRect, GraphicsUnit.Pixel);
            }

            return croppedImage;
        }
        static string GetImageAndBuildPalace()
        {
            List<string> images = new List<string>(Directory.GetFiles(InProcessDirectory));

            bool emailTime = true;
            string dateTaken = "", timeTaken = "";

            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("dd/MM/yyyy HH:mm:ss");

            // Split the input string by space
            string[] parts = formattedDateTime.Split(' ');

            // Extract the date and time parts
            dateTaken = parts[0]; // "10/21/2023"
            timeTaken = parts[1]; // "11:09:58"

            foreach (string str in images)
            {
                // Load the Picture
                using (Image image = Image.FromFile(str))
                {
                    if (image.PropertyIdList.Contains(0x9003))
                    {
                        emailTime = false;

                        PropertyItem propItem = image.GetPropertyItem(0x9003);
                        string inputDate = Encoding.UTF8.GetString(propItem.Value);

                        SeparateDateAndTime(inputDate, out dateTaken, out timeTaken);
                        timeTaken = timeTaken.TrimEnd('\0');
                    }
                }
            }
            string nameOfTheSenderEmail = EmailSender.Substring(0, EmailSender.IndexOf('@'));
            string currentDirectory = Path.Combine(OutProcessDirectory, nameOfTheSenderEmail);//name of the email sender.
            if (!Directory.Exists(currentDirectory))
                Directory.CreateDirectory(currentDirectory);

            string[] dateParts = dateTaken.Split('/');
            string day = dateParts[0], month = dateParts[1], year = dateParts[2];

            currentDirectory = Path.Combine(currentDirectory, GetMonthNameHebrew(int.Parse(month)));//month of the time the image was takened or time of the image been send.
            if (!Directory.Exists(currentDirectory))
                Directory.CreateDirectory(currentDirectory);

            currentDirectory = Path.Combine(currentDirectory, day);//the day...
            if (!Directory.Exists(currentDirectory))
                Directory.CreateDirectory(currentDirectory);

            currentDirectory = Path.Combine(currentDirectory, FileLines[ClassIndexes[ClassNum]]);//the name of the class
            if (!Directory.Exists(currentDirectory))
                Directory.CreateDirectory(currentDirectory);

            string newImageLocation;
            string[] timeParts = timeTaken.Split(':');
            if (emailTime)
                newImageLocation = currentDirectory + "\\" + "Email Time, " + string.Join("-", timeParts) /*+ " " + TimeStamp(timeTaken)*/ + Path.GetExtension(images[0]);
            else
                newImageLocation = currentDirectory + "\\" + "Image Time, " + string.Join("-", timeParts) /*+ " " + TimeStamp(timeTaken)*/ + Path.GetExtension(images[0]);

            if (File.Exists(newImageLocation))
                File.Delete(newImageLocation);

            File.Move(images[0], newImageLocation);

            return newImageLocation;
            void SeparateDateAndTime(string inputDate, out string datePart, out string timePart)
            {
                string[] dateTimeParts = inputDate.Split(' ');

                if (dateTimeParts.Length >= 2)
                {
                    string[] dateComponents = dateTimeParts[0].Split(':'); // Split date by ':'

                    if (dateComponents.Length == 3)
                    {
                        datePart = $"{dateComponents[2]}/{dateComponents[1]}/{dateComponents[0]}"; // Reorder date components
                        timePart = dateTimeParts[1]; // Get the time part
                    }
                    else
                    {
                        // Handle the case where the date format is invalid
                        datePart = "";
                        timePart = "";
                    }
                }
                else
                {
                    // Handle the case where the input format is invalid
                    datePart = "";
                    timePart = "";
                }
            }
            /*string TimeStamp(string time)
            {
                string period = "";

                //if (timeParts.Length == 3)
                //{
                //    int hours = int.Parse(timeParts[0]);
                //    int minutes = int.Parse(timeParts[1]);

                //    if (hours == 7 && minutes >= 45 || hours == 8 && minutes < 30)
                //        period = "0";

                //    else if (hours == 8 && minutes >= 30 || hours == 9 && minutes < 15)
                //        period = "1";

                //    else if (hours == 9 && minutes >= 15 && minutes <= 59)
                //        period = "2";

                //    else if (hours == 10 && minutes >= 0 && minutes < 15)
                //        period = "Recess, after the 2th period";

                //    else if (hours == 10 && minutes >= 15 && minutes <= 59)
                //        period = "3";

                //    else if (hours == 11 && minutes >= 0 && minutes < 45)
                //        period = "4";

                //    else if (hours == 11 && minutes >= 45 || hours == 12 && minutes < 10)
                //        period = "Recess, after the 4th period";

                //    else if (hours == 12 && minutes >= 10 && minutes < 55)
                //        period = "5";

                //    else if (hours == 12 && minutes >= 55 || hours == 13 && minutes < 40)
                //        period = "6";

                //    else if (hours == 13 && minutes >= 40 || minutes < 55)
                //        period = "Recess, after the 6th period";

                //    else if (hours == 13 && minutes >= 55 || hours == 14 && minutes < 40)
                //        period = "7";

                //    else if (hours == 14 && minutes >= 40 || hours == 15 && minutes < 25)
                //        period = "8";

                //    else if (hours == 15 && minutes >= 25 && minutes < 35)
                //        period = "Recess, after the 8th period";

                //    else if (hours == 15 && minutes >= 35 || hours == 16 && minutes < 20)
                //        period = "9";

                //    else if (hours == 16 && minutes >= 20 || hours == 17 && minutes <= 5)
                //        period = "10";
                //}

                if (timeParts.Length == 3)
                {
                    int hours = int.Parse(timeParts[0]);
                    int minutes = int.Parse(timeParts[1]);

                    if ((hours == 7 && minutes >= 45) || (hours == 8 && minutes < 30))
                        period = "שעת אפס";

                    else if ((hours == 8 && minutes >= 30) || (hours == 9 && minutes < 15))
                        period = "שיעור 1";

                    else if (hours == 9 && minutes >= 15 && minutes <= 59)
                        period = "שיעור 2";

                    else if (hours == 10 && minutes >= 0 && minutes < 15)
                        period = "הפסקה, אחרי שיעור 2";

                    else if (hours == 10 && minutes >= 15 && minutes <= 59)
                        period = "שיעור 3";

                    else if (hours == 11 && minutes >= 0 && minutes < 45)
                        period = "שיעור 4";

                    else if ((hours == 11 && minutes >= 45) || (hours == 12 && minutes < 10))
                        period = "הפסקה, אחרי השיעור 4";

                    else if (hours == 12 && minutes >= 10 && minutes < 55)
                        period = "שיעור 5";

                    else if ((hours == 12 && minutes >= 55) || (hours == 13 && minutes < 40))
                        period = "שיעור 6";

                    else if ((hours == 13 && minutes >= 40) || hours == 13 && minutes < 55)
                        period = "הפסקה, אחרי השיעור 6";

                    else if ((hours == 13 && minutes >= 55) || (hours == 14 && minutes < 40))
                        period = "שיעור 7";

                    else if ((hours == 14 && minutes >= 40) || (hours == 15 && minutes < 25))
                        period = "שיעור 8";

                    else if (hours == 15 && minutes >= 25 && minutes < 35)
                        period = "הפסקה, אחרי השיעור 8";

                    else if ((hours == 15 && minutes >= 35) || (hours == 16 && minutes < 20))
                        period = "שיעור 9";

                    else if ((hours == 16 && minutes >= 20) || (hours == 17 && minutes <= 5))
                        period = "שיעור 10";
                }

                if (string.IsNullOrEmpty(period))// Handle the case where the time doesn't fall into any of the defined ranges
                    period = "לא בזמן בית הספר";

                return period;
            }*/
            string GetMonthNameHebrew(int monthNumber)
            {
                switch (monthNumber)
                {
                    case 1:
                        return "ינואר";
                    case 2:
                        return "פברואר";
                    case 3:
                        return "מרץ";
                    case 4:
                        return "אפריל";
                    case 5:
                        return "מאי";
                    case 6:
                        return "יוני";
                    case 7:
                        return "יולי";
                    case 8:
                        return "אוגוסט";
                    case 9:
                        return "ספטמבר";
                    case 10:
                        return "אוקטובר";
                    case 11:
                        return "נובמבר";
                    case 12:
                        return "דצמבר";
                    default:
                        return "חודש לא חוקי";
                }
                /*switch (monthNumber)
                {
                    case 1:
                        return "January";
                    case 2:
                        return "February";
                    case 3:
                        return "March";
                    case 4:
                        return "April";
                    case 5:
                        return "May";
                    case 6:
                        return "June";
                    case 7:
                        return "July";
                    case 8:
                        return "August";
                    case 9:
                        return "September";
                    case 10:
                        return "October";
                    case 11:
                        return "November";
                    case 12:
                        return "December";
                    default:
                        return "Invalid Month Number";
                }*/
            }

        }

        public static void SelectionSortBy_Y(Point[] arr)
        {
            int n = arr.Length;

            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;

                // Find the selectedIndex of the minimum element in the remaining unsorted portion
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j].Y < arr[minIndex].Y)
                    {
                        minIndex = j;
                    }
                }

                // Swap the found minimum element with the element at selectedIndex Y
                Point temp = arr[i];
                arr[i] = arr[minIndex];
                arr[minIndex] = temp;
            }
        }
        public static void SelectionSortBy_X(Point[] arr)
        {
            int n = arr.Length;

            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;

                // Find the selectedIndex of the minimum element in the remaining unsorted portion
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j].X < arr[minIndex].X)
                    {
                        minIndex = j;
                    }
                }

                // Swap the found minimum element with the element at selectedIndex Y
                Point temp = arr[i];
                arr[i] = arr[minIndex];
                arr[minIndex] = temp;
            }
        }
        public static void AlignRectangleCorners()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            // Find the minimum and maximum X and Y coordinates among the corners
            foreach (Point corner in Corners)
            {
                minX = Math.Min(minX, corner.X);
                minY = Math.Min(minY, corner.Y);
                maxX = Math.Max(maxX, corner.X);
                maxY = Math.Max(maxY, corner.Y);
            }

            // Create a new array of aligned corners
            Point[] alignedCorners = new Point[4];
            Corners[0] = new Point(minX, minY);
            Corners[1] = new Point(maxX, minY);
            Corners[2] = new Point(minX, maxY);
            Corners[3] = new Point(maxX, maxY);
        }

        static void RotateImageIfNecessaryForMatForImage(ref Image photo, string imagePath)
        {
            using (Image image = Image.FromFile(imagePath))
            {
                // Check for the orientation tag in the EXIF data
                foreach (PropertyItem propertyItem in image.PropertyItems)
                {
                    if (propertyItem.Id == 0x112)
                    {
                        // Get the orientation value
                        int orientation = BitConverter.ToUInt16(propertyItem.Value, 0);

                        // Rotate the image based on the orientation tag
                        switch (orientation)
                        {
                            case 3:
                                photo.RotateFlip(RotateFlipType.Rotate180FlipNone);// Rotate180
                                break;
                            case 6:
                                photo.RotateFlip(RotateFlipType.Rotate90FlipNone);// Rotate90CounterClockwise
                                break;
                            case 8:
                                photo.RotateFlip(RotateFlipType.Rotate270FlipNone);// Rotat90Clockwise
                                break;
                                // Add more cases as needed for other orientations
                        }
                    }
                }
            }
        }

        static void FindObjectsCorners(Mat image)
        {
            // TODO: consider dropping the line on the contour from the middle of the image instead.

            // TODO: make sure that the two points taht you find using the white color are on the same consecutive line.

            //for (int T = 0; T < bitmapImage.Height; T++)
            //{
            //    for (int j = 0; j < bitmapImage.Width; j++)
            //    {
            //        if (ColorDist(bitmapImage.GetPixel(j, T), Color.White, 40))
            //            bitmapImage.SetPixel(j, T, Color.White);
            //    }
            //}
            //bitmapImage.Save(OutputImageLocation);
            //System.Environment.Exit(0);

            Mat matImage = image.Clone();
            //matImage.Save(OutputImageLocation);
            //System.Environment.Exit(0);

            Mat grayImage = new Mat();
            CvInvoke.CvtColor(matImage, grayImage, ColorConversion.Bgr2Gray);
            //grayImage.Save(OutputImageLocation);
            //System.Environment.Exit(0);
            matImage.Dispose();


            Mat threshold = new Mat();
            // Apply Otsu's method for adaptive thresholding
            CvInvoke.Threshold(grayImage, threshold, 0, 255, ThresholdType.Otsu);


            //threshold.Save(OutputImageLocation);
            //System.Environment.Exit(0);
            grayImage.Dispose();

            // Apply morphological operations to enhance binary separation
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Erode(threshold, threshold, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(1));
            CvInvoke.Dilate(threshold, threshold, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(1));

            Bitmap thresholdBitmap = new Bitmap(threshold.ToBitmap());

            ErasingPointsThatTuchesTheFrame(thresholdBitmap);
            //thresholdBitmap.Save(OutputImageLocation);
            BitmapToMat(thresholdBitmap, threshold);
            thresholdBitmap.Dispose();


            Mat edges = new Mat();
            CvInvoke.Canny(threshold, edges, 100, 200);
            threshold.Dispose();



            // Apply morphological dilation to close gaps
            kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Dilate(edges, edges, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(1));
            kernel.Dispose();

            Bitmap blackAndWhiteCroppedImage = new Bitmap(edges.ToBitmap());
            //blackAndWhiteCroppedImage.Save(OutputImageLocation);
            //System.Environment.Exit(0);


            Bitmap paint = new Bitmap(blackAndWhiteCroppedImage);
            //blackAndWhiteCroppedImage.Save(OutputImageLocation);
            //System.Environment.Exit(0);


            Mat objectFilledWithColor = new Mat();
            BitmapToMat(blackAndWhiteCroppedImage, objectFilledWithColor);









            Point touchingPoint = new Point();
            int x = blackAndWhiteCroppedImage.Width / 2, y, i;
            Color currentColor = new Color();

            bool foundCorner = false;
            for (y = 0; y < blackAndWhiteCroppedImage.Height; y++)
            {
                currentColor = blackAndWhiteCroppedImage.GetPixel(x, y);
                paint.SetPixel(x, y, Color.Red);

                if (ColorDist(currentColor, Color.White))
                {
                    touchingPoint = new Point(x, y);
                    foundCorner = true;
                    break;
                }
            }



            blackAndWhiteCroppedImage.Dispose();
            //paint.Save(OutputImageLocation);
            //System.Environment.Exit(0);

            // Find contours in the edge image
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(edges, contours, null, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
            edges.Dispose();

            // Define two points on the object's contour (you can modify these)
            Point startPoint = touchingPoint;

            // Find the contour containing the startPoint
            int startPointContourIdx = FindClosestContour(contours, startPoint);

            if (startPointContourIdx >= 0)
            {
                // Draw the contour
                CvInvoke.DrawContours(objectFilledWithColor, contours, startPointContourIdx, new MCvScalar(0, 0, 255), -1);
            }
            //CvInvoke.Imwrite(OutputImageLocation, objectFilledWithColor);


            Bitmap objectFilledWithColorBitmap = objectFilledWithColor.ToImage<Bgr, Byte>().ToBitmap();
            //objectFilledWithColorBitmap.Save(OutputImageLocation);
            //System.Environment.Exit(0);
            objectFilledWithColor.Dispose();






            /*// Find Top Left Objects Corner.
            int x = 0, y, i;
            Color currentColor = new Color();

            bool foundCorner = false;
            do
            {
                for (i = 0, y = 0; i <= x && y < blackAndWhiteCroppedImage.Height - 1; i++, y++)
                {
                    currentColor = blackAndWhiteCroppedImage.GetPixel(x - i, y);
                    paint.SetPixel(x - i, y, Color.Red);

                    if (ColorDist(currentColor, Color.White))
                    {


                        Corners[0] = new Point(x - i, y);
                        foundCorner = true;
                        break;
                    }
                }
                x++;
            } while (x != blackAndWhiteCroppedImage.Width - 2 && !foundCorner);


            // Find Top Right Objects Corner.
            foundCorner = false;
            x = blackAndWhiteCroppedImage.Width - 1;
            do
            {
                for (i = 0, y = 0; x + i <= image.Width - 1 && y < blackAndWhiteCroppedImage.Height - 1; i++, y++)
                {
                    currentColor = blackAndWhiteCroppedImage.GetPixel(x + i, y);
                    paint.SetPixel(x + i, y, Color.Red);

                    if (ColorDist(currentColor, Color.White))
                    {


                        Corners[1] = new Point(x + i, y);
                        foundCorner = true;
                        break;
                    }
                }
                x--;
            } while (x != 0 && !foundCorner);
            blackAndWhiteCroppedImage.Dispose();
            //paint.Save(OutputImageLocation);
            //System.Environment.Exit(0);

            // Find contours in the edge image
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(edges, contours, null, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
            edges.Dispose();

            // Define two points on the object's contour (you can modify these)
            Point startPoint = Corners[0];
            Point endPoint = Corners[1];

            // Find the contour containing the startPoint
            int startPointContourIdx = FindClosestContour(contours, startPoint);

            if (startPointContourIdx >= 0)
            {
                // Draw the contour
                CvInvoke.DrawContours(objectFilledWithColor, contours, startPointContourIdx, new MCvScalar(0, 0, 255), -1);
            }
            //CvInvoke.Imwrite(OutputImageLocation, objectFilledWithColor);


            Bitmap objectFilledWithColorBitmap = objectFilledWithColor.ToImage<Bgr, Byte>().ToBitmap();
            //objectFilledWithColorBitmap.Save(OutputImageLocation);
            //System.Environment.Exit(0);
            objectFilledWithColor.Dispose();*/




            // Find Top Left Objects Corner.
            x = 0;
            currentColor = new Color();

            foundCorner = false;
            do
            {
                for (i = 0, y = 0; i <= x && y < objectFilledWithColorBitmap.Height - 1; i++, y++)
                {
                    currentColor = objectFilledWithColorBitmap.GetPixel(x - i, y);
                    paint.SetPixel(x - i, y, Color.Red);

                    if (ColorDist(currentColor, Color.Red))
                    {


                        Corners[0] = new Point(x - i, y);
                        foundCorner = true;
                        break;
                    }
                }
                x++;
            } while (x != objectFilledWithColorBitmap.Width - 2 && !foundCorner);


            // Find Top Right Objects Corner.
            foundCorner = false;
            x = objectFilledWithColorBitmap.Width - 1;
            do
            {
                for (i = 0, y = 0; x + i <= image.Width - 1 && y < objectFilledWithColorBitmap.Height - 1; i++, y++)
                {
                    currentColor = objectFilledWithColorBitmap.GetPixel(x + i, y);
                    paint.SetPixel(x + i, y, Color.Red);

                    if (ColorDist(currentColor, Color.Red))
                    {


                        Corners[1] = new Point(x + i, y);
                        foundCorner = true;
                        break;
                    }
                }
                x--;
            } while (x != 0 && !foundCorner);


            // For the bottom left corner
            x = 0;
            currentColor = new Color();
            foundCorner = false;
            do
            {
                for (i = 0, y = objectFilledWithColorBitmap.Height - 1; i <= x && y >= 0; i++, y--)
                {
                    currentColor = objectFilledWithColorBitmap.GetPixel(x - i, y);
                    paint.SetPixel(x - i, y, Color.Red);

                    if (ColorDist(currentColor, Color.Red))
                    {
                        Corners[2] = new Point(x - i, y);
                        foundCorner = true;
                        break;
                    }
                }
                x++;
            } while (x != objectFilledWithColorBitmap.Width - 2 && !foundCorner);

            foundCorner = false;

            // For the bottom right corner
            x = objectFilledWithColorBitmap.Width - 1;
            do
            {
                for (i = 0, y = objectFilledWithColorBitmap.Height - 1; x + i <= image.Width - 1 && y >= 0; i++, y--)
                {
                    currentColor = objectFilledWithColorBitmap.GetPixel(x + i, y);
                    paint.SetPixel(x + i, y, Color.Red);

                    if (ColorDist(currentColor, Color.Red))
                    {
                        Corners[3] = new Point(x + i, y);
                        foundCorner = true;
                        break;
                    }
                }
                x--;
            } while (x != 0 && !foundCorner);


            objectFilledWithColorBitmap.Dispose();

            //paint.Save(OutputImageLocation);
            //System.Environment.Exit(0);
            paint.Dispose();

            void ErasingPointsThatTuchesTheFrame(Bitmap img)
            {
                for (int P = 0; P < img.Width; P++)
                    img.SetPixel(P, 0, Color.White);
                for (int P = 0; P < img.Height; P++)
                    img.SetPixel(img.Width - 1, P, Color.White);
                for (int P = img.Width - 1; P > 0; P--)
                    img.SetPixel(P, img.Height - 1, Color.White);
                for (int p = img.Height - 1; p > 0; p--)
                    img.SetPixel(0, p, Color.White);
            }
        }
        // Find the index of the closest contour to a given touchingPoint
        static int FindClosestContour(VectorOfVectorOfPoint contours, Point point)
        {
            int closestContourIdx = -1;
            double minDistance = double.MaxValue;

            for (int i = 0; i < contours.Size; i++)
            {
                double distance = CvInvoke.PointPolygonTest(contours[i], point, true);

                if (distance >= 0 && distance < minDistance)
                {
                    minDistance = distance;
                    closestContourIdx = i;
                }
            }
            return closestContourIdx;
        }
        static Mat EnhanceImage(Mat img, double nudge)
        {
            // Define a white reference touchingPoint in the image (e.g., a region you want to be white)
            // You can define multiple reference points for different colors if needed
            MCvScalar whiteReference = CvInvoke.Mean(img, new Mat());

            // Define a scaling factor that nudges the enhancement
            // Adjust this value to control the enhancement

            // Calculate the scaling factor for each color channel with a nudge
            MCvScalar scalingFactor = new MCvScalar(255.0 / whiteReference.V0 * nudge, 255.0 / whiteReference.V1 * nudge, 255.0 / whiteReference.V2 * nudge);

            // Apply the scaling factor to the image to enhance colors
            Mat enhancedImage = new Mat();
            CvInvoke.ConvertScaleAbs(img, enhancedImage, scalingFactor.V0, 0);

            return enhancedImage;
        }
        static Bitmap EnhanceImage(Bitmap img, double nudge)
        {
            // Convert the input Bitmap to a Mat
            Mat enhancedMat = new Mat();
            BitmapToMat(img, enhancedMat);

            enhancedMat = EnhanceImage(enhancedMat, nudge);

            // Convert the enhanced Mat back to a Bitmap
            Bitmap enhancedBitmap = new Bitmap(enhancedMat.Width, enhancedMat.Height, PixelFormat.Format24bppRgb);
            enhancedBitmap = enhancedMat.ToImage<Bgr, Byte>().ToBitmap();

            return enhancedBitmap;
        }
        public static Point HorizontalLine(Point point1, Point point2, Bitmap image, int whichCornerTouchesFirst)
        {
            // Determine the start touchingPoint and end touchingPoint based on X-coordinates
            Point startPoint, endPoint;

            if (point1.X < point2.X)
            {
                startPoint = point1;
                endPoint = point2;
            }
            else
            {
                startPoint = point2;
                endPoint = point1;
            }
            bool startPointIsFirstCorner = Corners[whichCornerTouchesFirst] == startPoint;

            // Calculate the slope (incline) between the two points
            float slope = (float)(endPoint.Y - startPoint.Y) / (endPoint.X - startPoint.X);

            // Generate the mathematical equation of the line (y = mx + b)
            float intercept = startPoint.Y - slope * startPoint.X;

            // Iterate through points along the X-axis
            int y, x;
            for (x = startPoint.X; x <= endPoint.X; x++)
            {
                // Calculate the corresponding Y-coordinate using the equation
                y = (int)(slope * x + intercept);

                Line.Add(ColorDist(image.GetPixel(x, y), CornerColor));

                // Set the pixel color to draw the line
                //image.SetPixel(x, y, Color.Red);
            }
            if (startPointIsFirstCorner)
                x -= Line.Count() - FindLastTrueIndex(Line);
            else
            {
                x = startPoint.X;
                x += FindFirstTrueIndex(Line);
            }
            y = (int)(slope * x + intercept);

            return new Point(x, y);
        }
        public static Point VerticalLine(Point point1, Point point2, Bitmap image, int whichCornerTouchesFirst)
        {
            // Determine the start touchingPoint and end touchingPoint based on Y-coordinates
            Point startPoint, endPoint;

            if (point1.Y < point2.Y)
            {
                startPoint = point1;
                endPoint = point2;
            }
            else
            {
                startPoint = point2;
                endPoint = point1;
            }
            bool startPointIsFirstCorner = Corners[whichCornerTouchesFirst] == startPoint;

            // Calculate the slope (incline) between the two points
            float slope = (float)(endPoint.X - startPoint.X) / (endPoint.Y - startPoint.Y);

            // Generate the mathematical equation of the line (x = my + b)
            float intercept = startPoint.X - slope * startPoint.Y;

            // Iterate through points along the Y-axis
            int y, x;
            for (y = startPoint.Y; y <= endPoint.Y; y++)
            {
                // Calculate the corresponding X-coordinate using the equation
                x = (int)(slope * y + intercept);

                Line.Add(ColorDist(image.GetPixel(x, y), CornerColor));

                // Set the pixel color to draw the line
                //image.SetPixel(x, y, Color.Red);
            }
            if (startPointIsFirstCorner)
                y -= Line.Count() - FindLastTrueIndex(Line);
            else
            {
                y = startPoint.Y;
                y += FindFirstTrueIndex(Line);
            }
            x = (int)(slope * y + intercept);

            return new Point(x, y);
        }
        static int CountTrueValues(List<bool> boolList)
        {
            // Use the Count method with a lambda expression to count true values
            int count = boolList.Count(b => b == true); // You can also use b == true or simply b

            return count;
        }
        public static int FindLastTrueIndex(List<bool> list)
        {
            for (int i = list.Count() - 1; i >= 0; i--)
            {
                if (list[i])
                    return i;
            }

            // If no 'true' value is found, you can return a default value or throw an exception.
            // For example, return -1 to indicate that there are no 'true' values in the array.
            return -1;
        }
        public static int FindFirstTrueIndex(List<bool> list)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i])
                    return i;
            }

            // If no 'true' value is found, you can return a default value or throw an exception.
            // For example, return -1 to indicate that there are no 'true' values in the array.
            return -1;
        }

        static Mat PerformPerspectiveTransformation(Mat image, Point[] points)
        {
            using (Mat img = image.Clone())
            {
                if (img != null && points.Length == 4)
                {
                    PointF[] pts1 = new PointF[4];

                    for (int i = 0; i < 4; i++)
                    {
                        pts1[i] = new PointF(points[i].X, points[i].Y);
                    }

                    PointF[] pts2 = new PointF[]
                    {
                    new PointF(0, 0),
                    new PointF(image.Width - 1, 0),
                    new PointF(0, image.Height - 1),
                    new PointF(image.Width - 1, image.Height - 1)
                    };

                    using (Mat M = CvInvoke.GetPerspectiveTransform(pts1, pts2))
                    {
                        Mat dst = new Mat();
                        CvInvoke.WarpPerspective(img, dst, M, new Size(image.Width, image.Height));

                        return dst;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        static Bitmap BlackItOut(Mat image)
        {
            Mat grayImage = new Mat();
            CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

            //// Enhance image before finding rectangle for better accuracy.
            //enhancedImage = EnhanceImage(grayImage, 1);

            //// Apply Gaussian Blur with adjustable intensity
            //CvInvoke.GaussianBlur(grayImage, grayImage, new Size(blurIntensity, blurIntensity), 0);


            // Apply Otsu's method for adaptive thresholding
            CvInvoke.Threshold(grayImage, grayImage, 0, 255, ThresholdType.Otsu);
            //CvInvoke.Imwrite(OutputImageLocation, grayImage);



            // Find contours of your object
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(grayImage, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                grayImage.Dispose();

                // Create a mask for the object contours
                Mat mask = new Mat(image.Size, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255)); // Initialize the mask with white

                for (int i = 0; i < contours.Size; i++)
                {
                    CvInvoke.DrawContours(mask, contours, i, new MCvScalar(0), -1);
                }

                // Invert the mask to make the background black
                CvInvoke.BitwiseNot(mask, mask);

                // Create a black background image
                Mat blackBackground = new Mat(image.Size, DepthType.Cv8U, 3);
                blackBackground.SetTo(new MCvScalar(0, 0, 0)); // Initialize the background with black

                // Copy the object from the original image to the black background
                CvInvoke.BitwiseAnd(image, image, blackBackground, mask);
                mask.Dispose();

                // Save or display the resulting image
                //blackBackground.Save(OutputImageLocation);


                Bitmap blackBackgroundBitmap = blackBackground.ToImage<Bgr, Byte>().ToBitmap();
                blackBackground.Dispose();
                //blackBackgroundBitmap.Save(OutputImageLocation);



                for (int i = 0; i < blackBackgroundBitmap.Height; i++)
                {
                    for (int j = 0; j < blackBackgroundBitmap.Width; j++)
                    {
                        if (ColorDist(blackBackgroundBitmap.GetPixel(j, i), Color.White, 40))
                            blackBackgroundBitmap.SetPixel(j, i, Color.White);
                    }
                }

                //blackBackgroundBitmap.Save(OutputImageLocation);
                return blackBackgroundBitmap;
            }
        }
        static void CheckIfThePcketsAreBlueOrYello(Bitmap thresholded, ref Mat image, (int Width, int Height) SinglePocketImageSize)
        {
            //for (int x = 0; x < bitmap.Width; x++)
            //{
            //    bitmap.SetPixel(x, (int) (SinglePocketImageSize.Height / 16.0 * 3), Color.Red);
            //}
            //bitmap.Save(OutputImageLocation);

            bool found = false;
            for (int y = 0; y <= SinglePocketImageSize.Height / 16.0 * 3 && !found; y++)
            {
                for (int x = 0; x < thresholded.Width; x++)
                {
                    if (ColorDist(thresholded.GetPixel(x, y), YellowBackgroundPocketColor, 50))
                    {
                        found = true;
                        break;
                    }
                }
            }

            //Bitmap bitmapImage = BlackItOutWithoutWhite(image).ToImage<Bgr, Byte>().ToBitmap();

            //image.Save(OutputImageLocation);

            if (found)
            {
                Mat blueImage = new Mat();
                CvInvoke.CvtColor(image, blueImage, ColorConversion.Bgr2Rgb);


                // Convert BGR to HSV
                Mat hsvImage = new Mat();
                CvInvoke.CvtColor(blueImage, hsvImage, ColorConversion.Bgr2Hsv);

                // Split the channels (Hue, Saturation, Value)
                VectorOfMat channels = new VectorOfMat();
                CvInvoke.Split(hsvImage, channels);

                // Increase saturation by a factor (e.g., 1.5)
                CvInvoke.ConvertScaleAbs(channels[1], channels[1], 2, 0);

                // Increase lightness by a factor (e.g., 1.2)
                CvInvoke.ConvertScaleAbs(channels[2], channels[2], 1.7, 0);

                // Increase hue by an offset (e.g., 20 degrees)
                Mat hueOffset = new Mat(channels[0].Size, channels[0].Depth, channels[0].NumberOfChannels);
                CvInvoke.AddWeighted(channels[0], 1.0, hueOffset, 0.0, 3, channels[0]);

                // Merge the channels back to the HSV image
                CvInvoke.Merge(channels, hsvImage);

                // Convert back to the original color space (BGR)
                Mat adjustedImage = new Mat();
                CvInvoke.CvtColor(hsvImage, adjustedImage, ColorConversion.Hsv2Bgr);

                //adjustedImage.Save(OutputImageLocation);
                image = adjustedImage.Clone();
            }
        }

        static int CountNumber(ref Bitmap thresholded, (int Width, int Height) SinglePocketImageSize)
        {
            // TODO: Make this function more trus worthy.

            List<int> Ys = new List<int>();
            List<List<Point>> whitePointsMetrix = new List<List<Point>>();//TODO: try to find the start touchingPoint of the middle of the cells so you can check even some cut image

            int count = 0, jumps = SinglePocketImageSize.Height / 2;
            bool foundInCell = true, sequence = false;

            int y, x, NumOfFoundCells;
            for (y = thresholded.Height - 1; y >= SinglePocketImageSize.Height / 2; y--)// with the new algorithm might be able to make y = image.Height
            {
                NumOfFoundCells = 1;
                foundInCell = true;

                for (; foundInCell && NumOfFoundCells != NumPerPocketColumn * 2 * 2;)
                {
                    foundInCell = false;

                    for (x = thresholded.Width / (NumPerPocketColumn * 2 * 2) * NumOfFoundCells; x < thresholded.Width / (NumPerPocketColumn * 2) * (NumOfFoundCells + 1) && x < thresholded.Width - thresholded.Width / (NumPerPocketColumn * 2); x++)
                    {
                        if (ColorDist(thresholded.GetPixel(x, y), Color.White, 30))
                        {
                            NumOfFoundCells++;
                            foundInCell = true;
                            break;
                        }
                        //thresholded.SetPixel(x, y, Color.Red);
                    }
                }
                if (NumOfFoundCells >= NumPerPocketColumn * 2 * 2 - 4)
                    sequence = true;
                else
                {
                    if (Ys.Count() > 5)
                    {
                        count++;
                        //jumps = Ys.First() - Ys.Last();
                        y -= jumps;

                        foreach (int Y in Ys)
                        {
                            //for (int T = 0; T < thresholded.Width; T++)// Draw a line from the edge of the image to the edge of the object
                            //    thresholded.SetPixel(T, Y, Color.Red);

                            //for (int T = 0; T < thresholded.Height; T++)
                            //    thresholded.SetPixel(x, T, Color.Green);
                        }

                    }
                    Ys.Clear();
                    sequence = false;
                }

                if (sequence)
                    Ys.Add(y);
            }
            return count;
        }
        static bool[,] CountPhones(ref Bitmap image, (int Width, int Height) singlePocketImageSize)
        {
            PhoneNetwork = new bool[NumPerPocketRow, NumPerPocketColumn];
            List<int> Ys = new List<int>();

            int count = 0, jumps = image.Width / (NumPerPocketColumn * 2);
            bool sequence = false;

            int y = 0, x, column = -1, row;
            for (x = jumps; column != NumPerPocketColumn - 1; x += jumps * 2)
            {
                column++;

                for (y = 0; y < image.Height; y++)
                {
                    if (ColorDist(image.GetPixel(x, y), YellowBackgroundPocketColor, 50) && !ColorDist(image.GetPixel(x, y), Color.White, 5))// try to match between white and yellow
                        sequence = true;

                    else
                    {
                        if (Ys.Count() > 2)
                        {
                            count++;
                            y += singlePocketImageSize.Height / 2;
                            row = Ys.Last() / (image.Height / NumPerPocketRow);
                            PhoneNetwork[row, column] = sequence;

                            foreach (int Y in Ys)
                            {
                                //for (int T = 0; T < imageWithGrids.Width; T++)// Draw a column from the edge of the imageWithGrids to the edge of the object
                                //image.SetPixel(x, Y, Color.Red);
                            }

                            Ys.Clear();
                        }
                        sequence = false;
                    }

                    if (sequence)
                        Ys.Add(y);
                }
            }
            //Console.WriteLine("Number of pockets: " + count);
            return PhoneNetwork;
        }

        // Function to crop an image based on an array of PointF coordinates
        private static Bitmap CropImage(Bitmap originalImage, Point[] corners)
        {
            int minX = corners.Min(p => p.X);
            int minY = corners.Min(p => p.Y);
            int maxX = corners.Max(p => p.X);
            int maxY = corners.Max(p => p.Y);

            int width = maxX - minX;
            int height = maxY - minY;

            Rectangle cropRect = new Rectangle(minX, minY, width, height);

            Bitmap croppedImage = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(originalImage, 0, 0, cropRect, GraphicsUnit.Pixel);
            }

            return croppedImage;
        }
        // Function to crop an image based on an array of Point coordinates
        private static Mat CropImageToBlue(Mat originalImage, Point[] corners)
        {
            int minX = corners.Min(p => p.X);
            int minY = corners.Min(p => p.Y);
            int maxX = corners.Max(p => p.X);
            int maxY = corners.Max(p => p.Y);

            // Ensure that cropRect is within the bounds of originalImage
            int cropX = Math.Max(0, Math.Min(minX, originalImage.Width - 1));
            int cropY = Math.Max(0, Math.Min(minY, originalImage.Height - 1));
            int cropWidth = Math.Max(1, Math.Min(maxX - minX, originalImage.Width - cropX));
            int cropHeight = Math.Max(1, Math.Min(maxY - minY, originalImage.Height - cropY));

            Rectangle cropRect = new Rectangle(cropX, cropY, cropWidth, cropHeight);

            // Create a region of interest (ROI) using the corrected crop rectangle
            Mat roi = new Mat(originalImage, cropRect);

            // Clone the ROI to get the cropped image
            Mat croppedImage = new Mat();
            CvInvoke.CvtColor(roi, croppedImage, ColorConversion.Bgr2Rgb);

            return croppedImage;
        }
        // Function to crop an image based on an array of Point coordinates
        private static Mat CropImage(Mat originalImage, Point[] corners)
        {
            int minX = corners.Min(p => p.X);
            int minY = corners.Min(p => p.Y);
            int maxX = corners.Max(p => p.X);
            int maxY = corners.Max(p => p.Y);

            // Ensure that cropRect is within the bounds of originalImage
            int cropX = Math.Max(0, Math.Min(minX, originalImage.Width - 1));
            int cropY = Math.Max(0, Math.Min(minY, originalImage.Height - 1));
            int cropWidth = Math.Max(1, Math.Min(maxX - minX, originalImage.Width - cropX));
            int cropHeight = Math.Max(1, Math.Min(maxY - minY, originalImage.Height - cropY));

            Rectangle cropRect = new Rectangle(cropX, cropY, cropWidth, cropHeight);

            // Create a region of interest (ROI) using the corrected crop rectangle
            Mat roi = new Mat(originalImage, cropRect);

            // Clone the ROI to get the cropped image
            Mat croppedImage = new Mat();
            roi.CopyTo(croppedImage);

            return croppedImage;
        }
        // Function that converts between Bitmap to Mat
        static void BitmapToMat(Bitmap bitmap, Mat mat)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            CvInvoke.CvtColor(new Mat(bmpData.Height, bmpData.Width, DepthType.Cv8U, 3, bmpData.Scan0, bmpData.Stride), mat, ColorConversion.Bgr2Bgra);
            bitmap.UnlockBits(bmpData);
        }
        static void BitmapToMatWithRotation(Bitmap bitmap, Mat mat)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Mat tempMat = new Mat(bmpData.Height, bmpData.Width, DepthType.Cv8U, 3, bmpData.Scan0, bmpData.Stride);
            CvInvoke.Transpose(tempMat, mat);
            CvInvoke.Flip(mat, mat, FlipType.Horizontal);
            bitmap.UnlockBits(bmpData);
        }
        static bool ColorDist(Color c1, Color c2)
        {
            return CalculateCIE76ColorDifference(c1.R, c1.G, c1.B, c2.R, c2.G, c2.B) <= ColorDistLimit;
        }
        static bool ColorDist(Color c1, Color c2, int colorDistLimit)
        {
            return CalculateCIE76ColorDifference(c1.R, c1.G, c1.B, c2.R, c2.G, c2.B) <= colorDistLimit;
        }
        static int ColorDistInt(Color c1, Color c2)
        {
            return (int)Math.Round(CalculateCIE76ColorDifference(c1.R, c1.G, c1.B, c2.R, c2.G, c2.B));
        }
        public static double CalculateCIE76ColorDifference(int r1, int g1, int b1, int r2, int g2, int b2)
        {
            // Convert the RGB values to the Lab color space
            double[] lab1 = RGBToLab(r1, g1, b1);
            double[] lab2 = RGBToLab(r2, g2, b2);

            // Calculate the CIE76 color difference
            double deltaL = lab1[0] - lab2[0];
            double deltaa = lab1[1] - lab2[1];
            double deltab = lab1[2] - lab2[2];

            return Math.Sqrt(deltaL * deltaL + deltaa * deltaa + deltab * deltab);
        }
        private static double[] RGBToLab(int R, int G, int B)
        {
            // Convert RGB to XYZ color space
            double rLinear = R / 255.0;
            double gLinear = G / 255.0;
            double bLinear = B / 255.0;

            rLinear = (rLinear > 0.04045) ? Math.Pow((rLinear + 0.055) / 1.055, 2.4) : rLinear / 12.92;
            gLinear = (gLinear > 0.04045) ? Math.Pow((gLinear + 0.055) / 1.055, 2.4) : gLinear / 12.92;
            bLinear = (bLinear > 0.04045) ? Math.Pow((bLinear + 0.055) / 1.055, 2.4) : bLinear / 12.92;

            double x = rLinear * 0.4124564 + gLinear * 0.3575761 + bLinear * 0.1804375;
            double y = rLinear * 0.2126729 + gLinear * 0.7151522 + bLinear * 0.0721750;
            double z = rLinear * 0.0193339 + gLinear * 0.1191920 + bLinear * 0.9503041;

            // Convert XYZ to Lab
            x /= 0.950456;
            y /= 1.0;
            z /= 1.088754;

            x = (x > 0.008856) ? Math.Pow(x, 1.0 / 3.0) : (903.3 * x + 16.0) / 116.0;
            y = (y > 0.008856) ? Math.Pow(y, 1.0 / 3.0) : (903.3 * y + 16.0) / 116.0;
            z = (z > 0.008856) ? Math.Pow(z, 1.0 / 3.0) : (903.3 * z + 16.0) / 116.0;

            double L = Math.Max(0, 116.0 * y - 16.0);
            double a = (x - y) * 500.0;
            double b = (y - z) * 200.0;

            return new double[] { L, a, b };
        }

        static bool[] CutArrayPerClass(bool[] array)
        {
            List<bool> croppedArray = new List<bool>();

            for (int i = array.Length - 1; i >= 0; i--)
                if (i <= Names.Count() - 1)
                    croppedArray.Add(array[i]);
            croppedArray.Reverse();

            //List<int> falseCroppedArray = new List<int>();

            //for (int y = array[1].Length - 1; y >= 0; y--)
            //    if (array[1][y] <= Names.Count() - 1)
            //        falseCroppedArray.Add(array[1][y]);
            //falseCroppedArray.Reverse();

            //int[][] croppedArray = { croppedArray.ToArray(), falseCroppedArray.ToArray() };

            return croppedArray.ToArray();
        }

        static string ReverseString(string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        static void MakeSpaces(ref string str)
        {
            int length = str.Length;
            for (int i = 0; i < CMD_width - length; i++)
                str = " " + str;
        }
    }
}
