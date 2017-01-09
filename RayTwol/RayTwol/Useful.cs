using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace RayTwol
{
    public struct Colour
    {
        public byte r;
        public byte g;
        public byte b;

        public Colour(byte r = 0, byte g = 0, byte b = 0)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        static Random rnd = new Random();
        public static Colour RandomColour()
        {
            byte R = (byte)rnd.Next(40, 255);
            byte G = (byte)rnd.Next(40, 255);
            byte B = (byte)rnd.Next(40, 255);
            return new Colour(R, G, B);
        }

        public static Colour RandomColourSeeded(int seed)
        {
            rnd = new Random(seed);
            byte R = (byte)rnd.Next(40, 255);
            byte G = (byte)rnd.Next(40, 255);
            byte B = (byte)rnd.Next(40, 255);
            return new Colour(R, G, B);
        }
    }
    


    public static class Func
    {
        public static string CodeToGameName(string codeName)
        {
            switch (codeName)
            {
                case "Menu":
                    return "Main Menu";
                case "Mapmonde":
                    return "The Hall of Doors";
                case "Jail_10":
                    return "Introduction Cutscene";
                case "Jail_20":
                    return "Introduction - Jail";
                case "Learn_10":
                    return "The Woods of Light";
                case "Learn_30":
                    return "The Fairy Glade #1";
                case "Learn_31":
                    return "The Fairy Glade #2";
                case "Bast_20":
                    return "The Fairy Glade #3";
                case "Bast_22":
                    return "The Fairy Glade #4";
                case "Learn_60":
                    return "The Fairy Glade #5";
                case "Ski_10":
                    return "The Marshes of Awakening #1";
                case "Ski_60":
                    return "The Marshes of Awakening #2";
                case "Chase_10":
                    return "The Bayou #1";
                case "Chase_22":
                    return "The Bayou #2";
                case "Water_10":
                    return "The Sanctuary of Water and Ice #1";
                case "Water_20":
                    return "The Sanctuary of Water and Ice #2";
                case "Rodeo_10":
                    return "The Menhir Hills #1";
                case "Rodeo_40":
                    return "The Menhir Hills #2";
                case "Rodeo_60":
                    return "The Menhir Hills #3";
                case "Vulca_10":
                    return "The Cave of Bad Dreams #1";
                case "Vulca_20":
                    return "The Cave of Bad Dreams #2";
                case "GLob_30":
                    return "The Canopy #1";
                case "GLob_10":
                    return "The Canopy #2";
                case "GLob_20":
                    return "The Canopy #3";
                case "Whale_00":
                    return "Whale Bay #1";
                case "Whale_05":
                    return "Whale Bay #2";
                case "Whale_10":
                    return "Whale Bay #3";
                case "Plum_00":
                    return "The Sanctuary of Stone and Fire #1";
                case "Plum_20":
                    return "The Sanctuary of Stone and Fire - temple";
                case "Plum_10":
                    return "The Sanctuary of Stone and Fire #2";
                case "Bast_09":
                    return "The Echoing Caves - opening cutscene";
                case "Bast_10":
                    return "The Echoing Caves #1";
                case "Cask_10":
                    return "The Echoing Caves #2";
                case "Cask_30":
                    return "The Echoing Caves #3";
                case "Nave_10":
                    return "The Precipice #1";
                case "Nave_15":
                    return "The Precipice #2";
                case "Nave_20":
                    return "The Precipice #3";
                case "Seat_10":
                    return "The Top of the World #1";
                case "Seat_11":
                    return "The Top of the World #2";
                case "Earth_10":
                    return "The Sanctuary of Rock and Lava #1";
                case "Earth_20":
                    return "The Sanctuary of Rock and Lava #2";
                case "Earth_30":
                    return "The Sanctuary of Rock and Lava #3";
                case "Helic_10":
                    return "Beneath the Sanctuary of Rock and Lava #1";
                case "Helic_20":
                    return "Beneath the Sanctuary of Rock and Lava #2";
                case "Helic_30":
                    return "Beneath the Sanctuary of Rock and Lava #3";
                case "Morb_00":
                    return "Tomb of the Ancients #1";
                case "Morb_10":
                    return "Tomb of the Ancients #2";
                case "Morb_20":
                    return "Tomb of the Ancients #3";
                case "Learn_40":
                    return "The Iron Mountains #1";
                case "Ball":
                    return "The Iron Mountains - balloon cutscene";
                case "Ile_10":
                    return "The Iron Mountains #2";
                case "Mine_10":
                    return "The Iron Mountains part 3";
                case "Boat01":
                    return "The Prison Ship #1";
                case "Boat02":
                    return "The Prison Ship #2";
                case "Astro_00":
                    return "The Prison Ship #3";
                case "Astro_10":
                    return "The Prison Ship #4";
                case "Rhop_10":
                    return "The Crow's Nest";
                case "Ly_10":
                    return "The Walk of Life";
                case "Ly_20":
                    return "The Walk of Power";
                case "Bonux":
                    return "Bonus game";
                case "Nego_10":
                    return "Cine - Council Chamber of the Teensies";
                case "Poloc_10":
                    return "Cine - Polokus #1";
                case "Poloc_20":
                    return "Cine - Polokus #2";
                case "Poloc_30":
                    return "Cine - Polokus #3";
                case "Poloc_40":
                    return "Cine - Polokus #4";
                case "Batam_10":
                    return "Cine - Meanwhile on The Prison Ship";
                case "Batam_20":
                    return "Cine - Razorbeard buys the Grolgoth";
                case "Raycap":
                    return "Level scores";
                case "End_10":
                    return "Cine - Ending";
                case "Staff_10":
                    return "Cine - Credits";
                default:
                    return codeName;
            }
        }
        public static float Byte4ToFloat(byte b1, byte b2, byte b3, byte b4)
        {
            return BitConverter.ToSingle(new byte[4] { b1, b2, b3, b4 }, 0);
        }
        public static byte[] FloatToByte4(float fl)
        {
            return BitConverter.GetBytes(fl);
        }
        
        public static bool CheckIfTransparent(Bitmap image)
        {
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                    if (image.GetPixel(x, y).A != 255)
                        return true;
            return false;
        }
        
        public static byte[] DecryptSNA(FileInfo levelFile, string suffix = "")
        {
            var stream = new EncodedStream(File.ReadAllBytes(levelFile.FullName + suffix));
            stream.Seek(4, SeekOrigin.Current);
            byte[] buff = new byte[stream.Length];
            buff[0] = 0x79;
            buff[1] = 0xCC;
            buff[2] = 0xB5;
            buff[3] = 0x6A;
            stream.Read(buff, 4, (int)stream.Length - 4);

            return buff;
        }
        public static byte[] EncryptSNA(byte[] levelFile)
        {
            var stream = new EncodedStream(levelFile);
            stream.Seek(4, SeekOrigin.Current);
            byte[] buff = new byte[stream.Length];
            buff[0] = 0x79;
            buff[1] = 0xCC;
            buff[2] = 0xB5;
            buff[3] = 0x6A;
            stream.Read(buff, 4, (int)stream.Length - 4);

            return buff;
        }

        public static void MoveDirectory(this DirectoryInfo source, DirectoryInfo target)
        {
            if (!target.Exists)
                target.Create();

            foreach (var file in source.GetFiles())
                file.MoveTo(Path.Combine(target.FullName, file.Name));

            foreach (var subdir in source.GetDirectories())
                subdir.MoveDirectory(target.CreateSubdirectory(subdir.Name));
        }
        public static void DeleteDirectoryRecursive(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                DeleteDirectoryRecursive(directory);
                foreach (var file in new DirectoryInfo(directory).GetFiles())
                    file.Delete();
                Directory.Delete(directory, false);
            }
        }
        public static long DirectorySize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirectorySize(di);
            }
            return size;
        }

        public static void FirstTimeSetup()
        {
            var warn = new Warning("RayTwol", "Press OK to begin first-time setup. Once it finishes, please restart RayTwol.").ShowDialog();
            if (warn.Value)
            {
                if (Directory.Exists(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol"))
                    Func.DeleteDirectoryRecursive(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol");

                Directory.CreateDirectory(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol");
                Directory.CreateDirectory(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol\\Textures");

                var r2lib = new ProcessStartInfo("rayman2lib.exe");
                r2lib.Arguments = string.Format("exportallmaps \"{0}\" \"{1}\"", Editor.cf_gameDir + "\\Data\\World\\Levels", Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol");
                Process.Start(r2lib);
                r2lib.Arguments = string.Format("unpackcnt \"{0}\" \"{1}\" \"-png\"", Editor.cf_gameDir + "\\Data\\Textures.cnt", Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol\\Textures");
                Process.Start(r2lib);
            }
            Environment.Exit(0);
        }

        static int objStart = 0;
        static int objStartByte = 0;
        static bool objStartFound = false;
        static int objEnd = 0;
        static int objEndByte = 0;
        static bool objEndFound = false;
        static int entID;
        public static void FindObjects()
        {
            int n = 0;
            foreach (byte b in Editor.levelBytes)
            {
                // find object start
                if (!objStartFound)
                {
                    if (objStart == 0)
                        if (b == 0x06)
                            objStart++;
                        else
                            objStart = 0;

                    else if (objStart == 1)
                        if (b == 0x00)
                            objStart++;
                        else
                            objStart = 0;

                    else if (objStart == 2)
                        if (b == 0x00)
                            objStart++;
                        else
                            objStart = 0;

                    else if (objStart == 3)
                        if (b == 0x00)
                            objStart++;
                        else
                            objStart = 0;

                    else if (objStart == 4)
                        if (b != 0x00)
                        {
                            objStartByte = n - 4;
                            objStart = 0;
                            objStartFound = true;
                        }
                        else
                            objStart = 0;
                }

                // find object end
                if (objStartFound && !objEndFound)
                {
                    if (objEnd == 0)
                        if (b == 0xCD)
                            objEnd++;
                        else
                            objEnd = 0;

                    else if (objEnd == 1)
                        if (b == 0xCD)
                            objEnd++;
                        else
                            objEnd = 0;

                    else if (objEnd == 2)
                        if (b == 0xCD)
                            objEnd++;
                        else
                            objEnd = 0;

                    else if (objEnd == 3)
                        if (b == 0xCD)
                        {
                            objEndByte = n + 4;
                            objEnd = 0;
                            objEndFound = true;
                        }
                        else
                            objEnd = 0;
                }

                // separate object
                if (objEndFound)
                {
                    byte[] newObj_code = new byte[objEndByte - objStartByte];
                    int[] newObj_offsets = new int[2];
                    CodeBlock newObj = new CodeBlock(newObj_code, objStartByte, objEndByte, newObj_offsets);

                    int objByte = 0;
                    for (int bb = objStartByte; bb < objEndByte; bb++)
                    {
                        newObj.code[objByte] = Editor.levelBytes[bb];
                        objByte++;
                    }

                    Editor.objects.Add(newObj);
                    objStartFound = false;
                    objEndFound = false;
                }
                n++;
            }

            entID = 0;
            foreach (CodeBlock ok in Editor.objects)
                CollectObjectInfo(ok);
            entID = 0;
        }
        

        public static void CollectObjectInfo(CodeBlock codeBlock)
        {
            if (CollectObjectInfo_Name(codeBlock) != "")
            {Editor.entities.Add(new Entity
                (
                    codeBlock,
                    entID,
                    CollectObjectInfo_Name(codeBlock),
                    CollectObjectInfo_Position(codeBlock)
                ));
                entID++;
            }
        }

        static string CollectObjectInfo_Name(CodeBlock codeBlock)
        {
            byte[] obj = codeBlock.code;
            int nameByteStartCount = 0;
            int nameByteStart = 0;
            string name = "";
            bool nameFound = false;
            for (int b = 0; b < obj.Length; b++)
            {
                if (nameByteStartCount == 0)
                    if (obj[b] == 0xFF)
                        nameByteStartCount++;
                    else
                        nameByteStartCount = 0;

                else if (nameByteStartCount == 1)
                    if (obj[b] == 0xFF)
                        nameByteStartCount++;
                    else
                        nameByteStartCount = 0;

                else if (nameByteStartCount == 2)
                    if (obj[b] == 0xFF)
                        nameByteStartCount++;
                    else
                        nameByteStartCount = 0;

                else if (nameByteStartCount == 3)
                    if (obj[b] == 0xFF)
                        nameByteStartCount++;
                    else
                        nameByteStartCount = 0;

                else if (nameByteStartCount == 4)
                    if (obj[b] == 0xFF)
                        nameByteStartCount++;
                    else
                        nameByteStartCount = 0;

                else if (nameByteStartCount == 5)
                    if (obj[b] == 0xFF)
                        nameByteStartCount++;
                    else
                        nameByteStartCount = 0;

                else if (nameByteStartCount == 6)
                    if (obj[b] == 0xFF)
                        nameByteStartCount++;
                    else
                        nameByteStartCount = 0;

                else if (nameByteStartCount == 7)
                    if (obj[b] == 0xFF)
                        nameByteStartCount++;
                    else
                        nameByteStartCount = 0;

                else if (nameByteStartCount == 8)
                    if (obj[b] == 0x00)
                    {
                        nameByteStart = b + 0x80;
                        nameByteStartCount = 0;
                        nameFound = true;
                    }
                    else
                        nameByteStartCount = 0;


                if (nameFound)
                {
                    name = "";
                    try
                    {
                        for (int nb = nameByteStart; obj[nb] != 0x00; nb++)
                        {
                            name += Convert.ToChar(obj[nb]);
                        }
                    }
                    catch { }

                    nameFound = false;
                }
                else
                {
                    nameFound = false;
                }
            }
            codeBlock.offsets[0] = nameByteStart;
            return name;
        }
        static Vec3 CollectObjectInfo_Position(CodeBlock codeBlock)
        {
            byte[] obj = codeBlock.code;
            int posOff = 0;
            Vec3 pos;

            try
            {
                posOff = 0x74;
                if (obj[posOff - 2] == 0x00 &&
                    obj[posOff - 1] == 0x00 &&
                    obj[posOff + 12] == 0x00 &&
                    obj[posOff + 13] == 0x00)
                    pos = new Vec3
                    (
                        Byte4ToFloat(obj[posOff + 0], obj[posOff + 1], obj[posOff + 2], obj[posOff + 3]),
                        Byte4ToFloat(obj[posOff + 4], obj[posOff + 5], obj[posOff + 6], obj[posOff + 7]),
                        Byte4ToFloat(obj[posOff + 8], obj[posOff + 9], obj[posOff + 10], obj[posOff + 11])
                    );

                else
                    posOff = 0x80;
                if (obj[posOff - 2] == 0x00 &&
                    obj[posOff - 1] == 0x00 &&
                    obj[posOff + 12] == 0x00 &&
                    obj[posOff + 13] == 0x00)
                    pos = new Vec3
                    (
                        Byte4ToFloat(obj[posOff + 0], obj[posOff + 1], obj[posOff + 2], obj[posOff + 3]),
                        Byte4ToFloat(obj[posOff + 4], obj[posOff + 5], obj[posOff + 6], obj[posOff + 7]),
                        Byte4ToFloat(obj[posOff + 8], obj[posOff + 9], obj[posOff + 10], obj[posOff + 11])
                    );
                else
                    posOff = 0x98;
                if (obj[posOff - 2] == 0x00 &&
                    obj[posOff - 1] == 0x00 &&
                    obj[posOff + 12] == 0x00 &&
                    obj[posOff + 13] == 0x00)
                    pos = new Vec3
                    (
                        Byte4ToFloat(obj[posOff + 0], obj[posOff + 1], obj[posOff + 2], obj[posOff + 3]),
                        Byte4ToFloat(obj[posOff + 4], obj[posOff + 5], obj[posOff + 6], obj[posOff + 7]),
                        Byte4ToFloat(obj[posOff + 8], obj[posOff + 9], obj[posOff + 10], obj[posOff + 11])
                    );
                else
                    pos = new Vec3(0, 0, 0, false);
            }
            catch
            {
                pos = new Vec3(0, 0, 0, false);
            }

            if (posOff > 0)
                codeBlock.offsets[1] = posOff;

            if (pos.x < 2 && pos.x > -2 &&
                pos.y < 2 && pos.y > -2 &&
                pos.z < 2 && pos.z > -2)
                return new Vec3(0, 0, 0, false);
            else
                return pos;
        }


        public static void ResetLevelLoadValues()
        {
            objStart = 0;
            objStartByte = 0;
            objStartFound = false;
            objEnd = 0;
            objEndByte = 0;
            objEndFound = false;
        }
    }


    public static class Editor
    {
        public static StreamReader configRead;
        public static List<FileInfo> levelFiles = new List<FileInfo>();
        public static List<FileInfo> levelFilesOriginal = new List<FileInfo>();
        public static FileInfo currLevel;
        public static byte[] levelBytes;
        public static List<CodeBlock> objects = new List<CodeBlock>();
        public static List<Entity> entities = new List<Entity>();

        public static char[] cfSplit = { '\t' };
        public static bool configFound;
        public static string cf_gameDir = null;

        public static EventHandler LevelLoad;
        public static EventHandler RaytwolInit;
        public static EventHandler ObjPosChanged;

        public static DirectoryInfo gameDir;
        public static DirectoryInfo[] levelDirs;

        public static void Init()
        {
            bool validSetup = false;

            while (!validSetup)
            {
                if (File.Exists("RayTwol.ini"))
                {
                    configRead = new StreamReader("RayTwol.ini");
                    configFound = true;
                }
                else
                    configFound = false;

                if (configFound)
                {
                    string line = configRead.ReadLine();

                    if (line.Split(cfSplit)[0] == "dir:")
                        cf_gameDir = line.Split(cfSplit)[2];

                    configRead.Close();
                }

                if (cf_gameDir == null)
                    InvalidDir();
                else
                {
                    try
                    {
                        gameDir = new DirectoryInfo(cf_gameDir + "\\Data\\World\\Levels");
                        levelDirs = new DirectoryInfo[0];
                        levelDirs = gameDir.GetDirectories("*", SearchOption.AllDirectories);
                        validSetup = true;
                    }
                    catch
                    {
                        InvalidDir();
                    }
                }
            }
            

            
            foreach (DirectoryInfo levelDir in levelDirs)
                try
                {
                    levelFiles.Add(levelDir.GetFiles(levelDir.Name + ".sna")[0]);
                }
                catch { }

            var viewport = new MainWindow();
            RaytwolInit(null, EventArgs.Empty);
            viewport.Show();
        }
        public static void InvalidDir()
        {
            Warning warning = new Warning("Warning", "A valid Rayman 2 install directory has not been selected.");
            warning.ShowDialog();
            if (warning.DialogResult != true)
                Environment.Exit(0);
            else
            {
                var gamePathDialog = new FolderBrowserDialog();
                gamePathDialog.ShowDialog();
                cf_gameDir = gamePathDialog.SelectedPath;
                SaveConfig();
            }
        }
        public static void SaveConfig()
        {
            StreamWriter configWrite = new StreamWriter("RayTwol.ini");

            configWrite.WriteLine("dir:\t\t" + cf_gameDir);

            configWrite.Close();
        }
        
        public static void OpenLevel(FileInfo level, string suffix = "")
        {
            Cursor.Current = Cursors.WaitCursor;

            Global.selectedEntity = null;
            Global.mouseOverEntity = null;
            Global.ClearAllMeshes();
            Global.entityHandles.Clear();
            entities.Clear();
            objects.Clear();

            if (!new FileInfo(level.FullName + "_ORIG").Exists)
                level.CopyTo(level.FullName + "_ORIG");
            levelBytes = Func.DecryptSNA(level, suffix);
            
            Func.ResetLevelLoadValues();

            Global.LoadSceneFromFolder(cf_gameDir + "\\Data\\World\\Levels\\_raytwol\\" + level.Directory.Name);

            Func.FindObjects();

            currLevel = level;

            Cursor.Current = Cursors.Default;

            LevelLoad(null, EventArgs.Empty);
        }
        public static void SaveLevel(string suffix = "")
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (Entity ent in Editor.entities)
            {
                ent.UpdateCode();

                for (int b = ent.code.start; b < ent.code.end; b++)
                    levelBytes[b] = ent.code.code[b - ent.code.start];

                FileStream sna = new FileStream(currLevel.FullName + suffix, FileMode.OpenOrCreate);
                sna.Write(Func.EncryptSNA(levelBytes), 0, levelBytes.Length);
                sna.Close();
            }
            Cursor.Current = Cursors.Default;
        }

        public static void CreateDisplayLists()
        {

        }
    }

    
    // by: szymski
    public class EncodedStream : Stream
    {
        byte[] data;

        public uint magic = 1790299257;

        public EncodedStream(byte[] data)
        {
            this.data = data;
            CanRead = true;
            CanSeek = true;
            CanWrite = false;
            Length = data.Length;
            Position = 0;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                Position = offset;
            else
                Position += offset;
            return Position;
        }

        public long SeekWithUpdatedMagic(long offset)
        {
            Position += offset;

            for (int i = 0; i < offset; i++)
                magic = (uint)(16807 * (magic ^ 0x75BD924) - 0x7FFFFFFF * ((magic ^ 0x75BD924u) / 0x1F31D));

            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (long i = Position; i < Position + count; i++)
            {
                buffer[i - Position + offset] = data[i];
                buffer[i - Position + offset] ^= (byte)((magic >> 8) & 255);

                magic = (uint)(16807 * (magic ^ 0x75BD924) - 0x7FFFFFFF * ((magic ^ 0x75BD924u) / 0x1F31D));
            }

            Position += count;

            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }
    }
}
