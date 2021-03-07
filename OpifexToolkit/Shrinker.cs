using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpifexToolkit
{
    class Shrinker
    {
        public static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        public Tuple<string, string, string> GetResourceInfo(
            string VMFline
        )
        {
            if (CountStringOccurrences(VMFline, "\"") == 0)
            {
                VMFline = VMFline.Substring(VMFline.LastIndexOf(" ") + 1);
            }
            else
            {
                while (CountStringOccurrences(VMFline, "\"") > 1)
                {
                    VMFline = VMFline.Substring(VMFline.IndexOf("\"") + 1);
                }
                VMFline = VMFline.Substring(0, VMFline.IndexOf("\""));
            }

            string ResPath = "";
            string ResName = "";

            int subEnd = VMFline.LastIndexOf(@"/");
            if (subEnd != -1)
            {
                ResPath = VMFline.Substring(0, subEnd);
                ResName = VMFline.Substring(subEnd + 1).ToLower();
            }
            else
            {
                ResPath = "";
                ResName = VMFline;
            }

            Console.WriteLine(VMFline);
            Console.WriteLine(ResPath);
            Console.WriteLine(ResName);

            return Tuple.Create(VMFline, ResPath, ResName);
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public void DecompileModel(string directory, string args)
        {
            Directory.CreateDirectory(directory);

            Process vpkDecomp = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Directory.GetCurrentDirectory() + "\\" + "CrowbarCommandLineDecomp.exe",
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };
            vpkDecomp.Start();
            while (!vpkDecomp.StandardOutput.EndOfStream)
            {
                Console.WriteLine(vpkDecomp.StandardOutput.ReadToEnd());
            }
        }

        public Tuple<Dictionary<string, int>, Dictionary<string, int>> RecMat
        (
            Tuple<string, string, string> resInfo, 
            Dictionary<string, int> materialList, 
            Dictionary<string, int> textureList, 
            string CPPath, 
            string OutputPath
        )
        {
            string VMFline = resInfo.Item1;
            string OrigResPath = resInfo.Item2.ToLower();
            string ResPath = "materials/" + resInfo.Item2.ToLower();
            string ResName = resInfo.Item3.ToLower();


            if (materialList.ContainsKey(OrigResPath + "/" + ResName) == false)
            {
                materialList[OrigResPath + "/" + ResName] = 1;
                if (Directory.Exists(CPPath + "\\" + ResPath.Replace("/", "\\")))
                {
                    if (File.Exists(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt"))
                    {
                        Directory.CreateDirectory(OutputPath + "\\" + ResPath.Replace("/", "\\"));
                        try
                        {
                            File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt", OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt", true);
                        }
                        catch (FileNotFoundException)
                        { }
                        System.IO.StreamReader vmt = new System.IO.StreamReader(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt");
                        string VMTline;
                        while ((VMTline = vmt.ReadLine()) != null)
                        {
                            VMTline = VMTline.Replace("\\", "/");
                            if (VMTline.ToLower().Contains("$basetexture") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false && VMTline.ToLower().Contains("$basetexturetransform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$bumpmap") || VMTline.ToLower().Contains("$basetexture2") && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$bumpmap2") || VMTline.ToLower().Contains("$blendmodulatetexture") || VMTline.ToLower().Contains("%tooltexture") || VMTline.ToLower().Contains("$envmapmask") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false || VMTline.ToLower().Contains("$detail") && VMTline.ToLower().Contains("$detailscale") == false && VMTline.ToLower().Contains("$detailblendfactor") == false && VMTline.ToLower().Contains("$detailblendmode") == false || VMTline.ToLower().Contains("$flowmap") || VMTline.ToLower().Contains("$flow_noise_texture") || VMTline.ToLower().Contains("$texture2") && VMTline.ToLower().Contains("$basetexture2") == false && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$phongexponenttexture") || VMTline.ToLower().Contains("$envmap") && VMTline.ToLower().Contains("$envmapmask") == false && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false)
                            {
                                //texture
                                VMTline = VMTline.Replace(".vtf", "");
                                Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                if (Directory.Exists(CPPath + "\\materials\\" + resInfoT.Item2.Replace("/", "\\")))
                                {
                                    if (File.Exists(CPPath + "\\materials\\" + resInfoT.Item1.Replace("/", "\\") + ".vtf"))
                                    {
                                        Directory.CreateDirectory(OutputPath + "\\materials\\" + resInfoT.Item2.Replace("/", "\\"));
                                        try
                                        {
                                            File.Copy(CPPath + "\\materials\\" + resInfoT.Item1.Replace("/", "\\") + ".vtf", OutputPath + "\\materials\\" + resInfoT.Item1.Replace("/", "\\") + ".vtf", true);
                                        }
                                        catch (FileNotFoundException)
                                        { }
                                    }
                                }

                                Console.WriteLine(VMTline);
                            }
                            else if (VMTline.ToLower().Contains("include"))
                            {
                                //material
                                Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                Tuple<Dictionary<string, int>, Dictionary<string, int>> response = RecMat(resInfoT, materialList, textureList, CPPath, OutputPath);
                                materialList = response.Item1;
                                textureList = response.Item2;

                            }
                            else if (VMTline.ToLower().Contains("$bottommaterial"))
                            {
                                Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                Tuple<Dictionary<string, int>, Dictionary<string, int>> response = RecMat(resInfoT, materialList, textureList, CPPath, OutputPath);
                                materialList = response.Item1;
                                textureList = response.Item2;

                            }
                            else if (VMTline.ToLower().Contains("$crackmaterial"))
                            {
                                //Only useful for maps with CSS glass, I will finish this handler at some point or if somebody asks nicely
                            }
                        }
                        vmt.Close();
                    }
                }
            }

            return Tuple.Create(materialList, textureList); ;
        }





        public Tuple<Dictionary<string, int>, Dictionary<string, int>, Dictionary<string, int>, Dictionary<string, int>> VMFHandler( //VMFline, CPPath, OutputPath, ResWhite, ResWhiteList, OutputPath
        string VMFline,
        string CPPath,
        string OutputPath,
        Dictionary<string, int> soundList,
        Dictionary<string, int> materialList,
        Dictionary<string, int> textureList,
        Dictionary<string, int> modelList
        )
        {
            string txtLine = VMFline;

            //Console.WriteLine(VMFline);

            if (VMFline.ToLower().Contains(".mp3") || VMFline.ToLower().Contains(".wav"))
            {
                //sound handler
                Tuple<string, string, string> resInfo = GetResourceInfo(VMFline);
                VMFline = resInfo.Item1;
                string OrigResPath = resInfo.Item2.ToLower();
                string ResPath = "sound/" + resInfo.Item2.ToLower();
                string ResName = resInfo.Item3.ToLower();
                if(soundList.ContainsKey(OrigResPath + "/" + ResName) == false)
                {
                    soundList[OrigResPath + "/" + ResName] = 1;
                    if (Directory.Exists(CPPath+"\\"+ ResPath.Replace("/","\\")))
                    {
                        if (File.Exists(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName))
                        {
                            Directory.CreateDirectory(OutputPath + "\\" + ResPath.Replace("/", "\\"));
                            try
                            {
                                File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName, OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName, true);
                            }
                            catch (FileNotFoundException)
                            { }
                        }
                    }
                }
            }
            else if (VMFline.Contains("material") && VMFline.Contains("detailmaterial") == false || VMFline.Contains("texture") && VMFline.Contains("TextureScale") == false)
            {
                //Material Handler (1st run)
                Tuple<string, string, string> resInfo = GetResourceInfo(VMFline);
                VMFline = resInfo.Item1;
                string OrigResPath = resInfo.Item2.ToLower();
                string ResPath = "materials/" + resInfo.Item2.ToLower();
                string ResName = resInfo.Item3.ToLower();


                if (materialList.ContainsKey(OrigResPath + "/" + ResName) == false)
                {
                    materialList[OrigResPath + "/" + ResName] = 1;
                    if (Directory.Exists(CPPath + "\\" + ResPath.Replace("/", "\\")))
                    {
                        if (File.Exists(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt"))
                        {
                            Directory.CreateDirectory(OutputPath + "\\" + ResPath.Replace("/", "\\"));
                            try
                            {
                                File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt", OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            System.IO.StreamReader vmt = new System.IO.StreamReader(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt");
                            string VMTline;
                            while ((VMTline = vmt.ReadLine()) != null)
                            {
                                VMTline = VMTline.Replace("\\", "/");
                                if (VMTline.ToLower().Contains("$basetexture") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false && VMTline.ToLower().Contains("$basetexturetransform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$bumpmap") || VMTline.ToLower().Contains("$basetexture2") && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$bumpmap2") || VMTline.ToLower().Contains("$blendmodulatetexture") || VMTline.ToLower().Contains("%tooltexture") || VMTline.ToLower().Contains("$envmapmask") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false || VMTline.ToLower().Contains("$detail") && VMTline.ToLower().Contains("$detailscale") == false && VMTline.ToLower().Contains("$detailblendfactor") == false && VMTline.ToLower().Contains("$detailblendmode") == false || VMTline.ToLower().Contains("$flowmap") || VMTline.ToLower().Contains("$flow_noise_texture") || VMTline.ToLower().Contains("$texture2") && VMTline.ToLower().Contains("$basetexture2") == false && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$phongexponenttexture") || VMTline.ToLower().Contains("$envmap") && VMTline.ToLower().Contains("$envmapmask") == false && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false)
                                {
                                    //texture
                                    VMTline = VMTline.Replace(".vtf", "");
                                    Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);
                                    string TVMTline = resInfoT.Item1;

                                    if (textureList.ContainsKey(TVMTline) == false)
                                    {
                                        textureList[OrigResPath + "/" + ResName] = 1;
                                        if (Directory.Exists(CPPath + "\\materials\\" + resInfoT.Item2.Replace("/", "\\")))
                                        {
                                            if (File.Exists(CPPath + "\\materials\\" + resInfoT.Item1.Replace("/", "\\") + ".vtf"))
                                            {
                                                Directory.CreateDirectory(OutputPath + "\\materials\\" + resInfoT.Item2.Replace("/", "\\"));
                                                try
                                                {
                                                    File.Copy(CPPath + "\\materials\\" + resInfoT.Item1.Replace("/", "\\") + ".vtf", OutputPath + "\\materials\\" + resInfoT.Item1.Replace("/", "\\") + ".vtf", true);
                                                }
                                                catch (FileNotFoundException)
                                                { }
                                            }
                                        }
                                    }

                                    Console.WriteLine(VMTline);
                                }
                                else if (VMTline.ToLower().Contains("include"))
                                {
                                    //material
                                    Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                    Tuple<Dictionary<string, int>, Dictionary<string, int>> response = RecMat(resInfoT, materialList, textureList, CPPath, OutputPath);
                                    materialList = response.Item1;
                                    textureList = response.Item2;

                                }
                                else if (VMTline.ToLower().Contains("$bottommaterial"))
                                {
                                    Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                    Tuple<Dictionary<string, int>, Dictionary<string, int>> response = RecMat(resInfoT, materialList, textureList, CPPath, OutputPath);
                                    materialList = response.Item1;
                                    textureList = response.Item2;

                                }
                                else if (VMTline.ToLower().Contains("$crackmaterial"))
                                {
                                    //Only useful for maps with CSS glass, I will finish this handler at some point or if somebody asks nicely
                                }
                            }
                            vmt.Close();
                        }
                    }
                }
            }
            else if (VMFline.Contains("model") && VMFline.ToLower().Contains(".mdl"))
            {
                //Model Handler (1st run)
                Tuple<string, string, string> resInfo = GetResourceInfo(VMFline);
                VMFline = resInfo.Item1;
                string OrigResPath = "";
                if (resInfo.Item2.ToLower().Length >= 7)
                {
                    OrigResPath = resInfo.Item2.ToLower().Substring(7);
                }
                //Console.WriteLine(OrigResPath);
                string ResPath = resInfo.Item2.ToLower();
                string ResName = resInfo.Item3.ToLower();

                if (modelList.ContainsKey(OrigResPath + "/" + ResName) == false)
                {
                    modelList[OrigResPath + "/" + ResName] = 1;
                    if (Directory.Exists(CPPath + "\\" + ResPath.Replace("/", "\\")))
                    {
                        if (File.Exists(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName))
                        {
                            Directory.CreateDirectory(OutputPath + "\\" + ResPath.Replace("/", "\\"));
                            try
                            {
                                File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName, OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName, true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                    File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".vvd"), OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".vvd"), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                    File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".sw.vvd"), OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".sw.vvd"), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                    File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".dx90.vtx"), OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".dx90.vtx"), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                    File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".dx80.vtx"), OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".dx80.vtx"), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                    File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".vtx"), OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".vtx"), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                    File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".sw.vtx"), OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".sw.vtx"), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                    File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".xbox.vtx"), OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".xbox.vtx"), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                    File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".phy"), OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".phy"), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            { 
                                DecompileModel(CPPath + "\\temp\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ""), "-p " + "\"" + CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + "\"" + " -o \"" + CPPath + "\\temp\\" + ResPath.Replace("/", "\\") + "\"");
                            }
                            catch (FileNotFoundException)
                            { }
                            //read decompiled model
                            System.IO.StreamReader mdl = new System.IO.StreamReader(CPPath + "\\temp\\" + ResPath.Replace("/", "\\") + "\\" + ResName.Replace(".mdl", ".qc"));

                            string MDLline;
                            bool skinFlag = false;
                            bool skinSetOnce = false;
                            List<string> curMaterialPaths = new List<string>();
                            while ((MDLline = mdl.ReadLine()) != null)
                            {
                                MDLline = MDLline.Replace("\\", "/");
                                //Console.WriteLine(MDLline);
                                if (MDLline.Contains("cdmaterials") && !MDLline.Contains("$cdmaterials \"\""))
                                {
                                    //get model path from something like $cdmaterials "models\props_foliage\" -> "materials\models\props_foliage\"
                                    Console.WriteLine(MDLline.Substring(MDLline.IndexOf("\"") + 1, MDLline.LastIndexOf("\"") - MDLline.IndexOf("\"")).Replace("\\", "/").ToLower());
                                    if (MDLline.Substring(MDLline.IndexOf("\"") + 1, MDLline.LastIndexOf("\"") - MDLline.IndexOf("\"")).Replace("\\", "/").ToLower().EndsWith("/\""))
                                    {
                                        curMaterialPaths.Add(MDLline.Substring(MDLline.IndexOf("\"") + 1, MDLline.LastIndexOf("\"") - MDLline.IndexOf("\"") - 2).Replace("\\", "/").ToLower());
                                    }
                                    else if (MDLline.Substring(MDLline.IndexOf("\"") + 1, MDLline.LastIndexOf("\"") - MDLline.IndexOf("\"")).Replace("\\", "/").ToLower().EndsWith("\""))
                                    {
                                        curMaterialPaths.Add(MDLline.Substring(MDLline.IndexOf("\"") + 1, MDLline.LastIndexOf("\"") - MDLline.IndexOf("\"") - 1).Replace("\\", "/").ToLower());
                                    }
                                }
                                if (MDLline.Contains("{") == false && MDLline.Contains("}"))
                                {
                                    //remove flag
                                    skinFlag = false;
                                }
                                if (MDLline.Contains("{") && MDLline.Contains("}"))
                                {
                                    //get material name and run material finder function
                                    if (skinFlag)
                                    {
                                        foreach (string curMaterialPath in curMaterialPaths)
                                        {
                                            string OrigMatPath = curMaterialPath.ToLower();
                                            string MatPath = "materials/" + OrigMatPath;

                                            string fMatName = MDLline.Substring(MDLline.IndexOf("{") + 1, MDLline.LastIndexOf("}") - MDLline.IndexOf("{") - 1).Replace("\\", "/").ToLower();
                                            List<string> MatNames = new List<string>();
                                            while (fMatName.IndexOf("\"") != -1)
                                            {
                                                string tMatName = fMatName.Substring(fMatName.IndexOf("\"") + 1);
                                                fMatName = tMatName.Substring(tMatName.IndexOf("\"") + 1);
                                                tMatName = tMatName.Substring(0, tMatName.IndexOf("\""));
                                                MatNames.Add(tMatName);
                                            }

                                            foreach (var MatName in MatNames)
                                            {
                                                string VMTline = OrigMatPath + "/" + MatName;


                                                Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                                Tuple<Dictionary<string, int>, Dictionary<string, int>> response = RecMat(resInfoT, materialList, textureList, CPPath, OutputPath);
                                                materialList = response.Item1;
                                                textureList = response.Item2;
                                            }
                                        }
                                    }
                                }
                                if (MDLline.Contains("skinfamilies"))
                                {
                                    //set flag
                                    foreach (string curMaterialPath in curMaterialPaths)
                                    {
                                        Console.WriteLine(curMaterialPath);
                                    }
                                    skinFlag = true;
                                    skinSetOnce = true;
                                }

                            }
                            mdl.Close();

                            if (skinSetOnce == false)
                            {
                                foreach (string curMaterialPath in curMaterialPaths)
                                {
                                    //no skins, find and use default
                                    Console.WriteLine(curMaterialPath);
                                    string OrigMatPath = curMaterialPath.ToLower();
                                    string MatPath = "materials/" + OrigMatPath;
                                    string MatName = ResName;
                                    Console.WriteLine(OrigMatPath);
                                    Console.WriteLine(MatPath);
                                    Console.WriteLine(MatName);
                                    string VMTline = OrigMatPath + "/" + MatName;

                                    Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                    Tuple<Dictionary<string, int>, Dictionary<string, int>> response = RecMat(resInfoT, materialList, textureList, CPPath, OutputPath);
                                    materialList = response.Item1;
                                    textureList = response.Item2;
                                }
                            }
                        }
                    }
                }
            }
            else if (VMFline.Contains("model") && VMFline.ToLower().Contains(".vmt"))
            {
                //Material Handler (1st run)
                Tuple<string, string, string> resInfo = GetResourceInfo(VMFline);
                VMFline = resInfo.Item1.Replace(".vmt","");
                string OrigResPath = resInfo.Item2.ToLower();
                string ResPath = "materials/" + resInfo.Item2.ToLower();
                string ResName = resInfo.Item3.ToLower().Replace(".vmt", "");


                if (materialList.ContainsKey(OrigResPath + "/" + ResName) == false)
                {
                    materialList[OrigResPath + "/" + ResName] = 1;
                    if (Directory.Exists(CPPath + "\\" + ResPath.Replace("/", "\\")))
                    {
                        if (File.Exists(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt"))
                        {
                            Directory.CreateDirectory(OutputPath + "\\" + ResPath.Replace("/", "\\"));
                            try
                            {
                                File.Copy(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt", OutputPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            System.IO.StreamReader vmt = new System.IO.StreamReader(CPPath + "\\" + ResPath.Replace("/", "\\") + "\\" + ResName + ".vmt");
                            string VMTline;
                            while ((VMTline = vmt.ReadLine()) != null)
                            {
                                VMTline = VMTline.Replace("\\", "/");
                                if (VMTline.ToLower().Contains("$basetexture") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false && VMTline.ToLower().Contains("$basetexturetransform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$bumpmap") || VMTline.ToLower().Contains("$basetexture2") && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$bumpmap2") || VMTline.ToLower().Contains("$blendmodulatetexture") || VMTline.ToLower().Contains("%tooltexture") || VMTline.ToLower().Contains("$envmapmask") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false || VMTline.ToLower().Contains("$detail") && VMTline.ToLower().Contains("$detailscale") == false && VMTline.ToLower().Contains("$detailblendfactor") == false && VMTline.ToLower().Contains("$detailblendmode") == false || VMTline.ToLower().Contains("$flowmap") || VMTline.ToLower().Contains("$flow_noise_texture") || VMTline.ToLower().Contains("$texture2") && VMTline.ToLower().Contains("$basetexture2") == false && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$texture2") && VMTline.ToLower().Contains("$basetexture2") == false && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$phongexponenttexture") || VMTline.ToLower().Contains("$envmap") && VMTline.ToLower().Contains("$envmapmask") == false && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false)
                                {
                                    //texture
                                    VMTline = VMTline.Replace(".vtf", "");
                                    Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                    if (textureList.ContainsKey(OrigResPath + "/" + ResName) == false)
                                    {
                                        textureList[OrigResPath + "/" + ResName] = 1;
                                        if (Directory.Exists(CPPath + "\\materials\\" + resInfoT.Item2.Replace("/", "\\")))
                                        {
                                            if (File.Exists(CPPath + "\\materials\\" + resInfoT.Item1.Replace("/", "\\") + ".vtf"))
                                            {
                                                Directory.CreateDirectory(OutputPath + "\\materials\\" + resInfoT.Item2.Replace("/", "\\"));
                                                try
                                                {
                                                    File.Copy(CPPath + "\\materials\\" + resInfoT.Item1.Replace("/", "\\") + ".vtf", OutputPath + "\\materials\\" + resInfoT.Item1.Replace("/", "\\") + ".vtf", true);
                                                }
                                                catch (FileNotFoundException)
                                                { }
                                            }
                                        }
                                    }

                                    Console.WriteLine(VMTline);
                                }
                                else if (VMTline.ToLower().Contains("include"))
                                {
                                    //material
                                    Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                    Tuple<Dictionary<string, int>, Dictionary<string, int>> response = RecMat(resInfoT, materialList, textureList, CPPath, OutputPath);
                                    materialList = response.Item1;
                                    textureList = response.Item2;

                                }
                                else if (VMTline.ToLower().Contains("$bottommaterial"))
                                {
                                    Tuple<string, string, string> resInfoT = GetResourceInfo(VMTline);

                                    Tuple<Dictionary<string, int>, Dictionary<string, int>> response = RecMat(resInfoT, materialList, textureList, CPPath, OutputPath);
                                    materialList = response.Item1;
                                    textureList = response.Item2;

                                }
                                else if (VMTline.ToLower().Contains("$crackmaterial"))
                                {
                                    //Only useful for maps with CSS glass, I will finish this handler at some point or if somebody asks nicely
                                }
                            }
                            vmt.Close();
                        }
                    }
                }
            }

            return Tuple.Create(soundList, materialList, textureList, modelList);
        }





        /*
        * 
        * Content Pack Shrinker - Thread/Thread Creator
        * 
        */

        public Thread ShrinkerThreaded( //VMFPath, CPPath, OutputPath, IncludePath, ResWhite, ResWhiteList,
            string VMFPath,
            string CPPath,
            string OutputPath,
            string IncludePath,
            MainWindow window
        )
        {
            var t = new Thread(() => ShrinkerThreadedReal(VMFPath, CPPath, OutputPath, IncludePath, window));
            t.Start();
            return t;
        }

        private void ShrinkerThreadedReal(
            string VMFPath,
            string CPPath,
            string OutputPath,
            string IncludePath,
            MainWindow window
        )
        {
            //lets keep track of time
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Build out list of supported source engine VPKs
            App.Current.Dispatcher.Invoke(() =>
            {
                window.ImportStatusText.Content = "Building New Resource Pack From VMF";
            });

            int VMFLines = 0;
            using (StreamReader r = new StreamReader(VMFPath))
            {
                while (r.ReadLine() != null) { VMFLines++; }
            }

            Dictionary<string, int> soundList = new Dictionary<string, int>();
            Dictionary<string, int> materialList = new Dictionary<string, int>();
            Dictionary<string, int> textureList = new Dictionary<string, int>();
            Dictionary<string, int> modelList = new Dictionary<string, int>();

            System.IO.StreamReader vmf = new System.IO.StreamReader(VMFPath);
            string VMFline;
            int VMFLineCounter = 0;
            while ((VMFline = vmf.ReadLine()) != null)
            {
                Tuple<Dictionary<string, int>, Dictionary<string, int>, Dictionary<string, int>, Dictionary<string, int>> VFRResponse = VMFHandler(VMFline, CPPath, OutputPath, soundList, materialList, textureList, modelList);

                soundList = VFRResponse.Item1;
                materialList = VFRResponse.Item2;
                textureList = VFRResponse.Item3;
                modelList = VFRResponse.Item4;

                VMFLineCounter++;
                App.Current.Dispatcher.Invoke(
                    (Action<int, int>)((VMFLineCounte, VMFLine) =>
                    {
                        window.ShrinkStatusText.Content = "Building New Resource Pack From VMF (" + VMFLineCounte.ToString() + "/" + VMFLine.ToString() + ")";
                        double CB = (double)VMFLineCounte / VMFLine;
                        window.ShrinkCurrentPB.Value = CB * (double)100;
                        window.ShrinkOverallPB.Value = (CB * (double)90);
                    }),
                    new object[] { VMFLineCounter, VMFLines }
                );
            }
            vmf.Close();

            App.Current.Dispatcher.Invoke(() =>
            {
                window.ImportOverallPB.Value = 90;
                window.ImportCurrentPB.Value = 0;
                window.ImportStatusText.Content = "Building New Resource Pack - Cleanup";
            });

            //cleanup stuff
            //delete temp folder
            if (Directory.Exists(CPPath + "\\temp\\"))
            {
                try
                {
                    Directory.Delete(CPPath + "\\temp", true);
                }
                catch (IOException) { }
                try
                {
                    Directory.Delete(CPPath + "\\temp");
                }
                catch (IOException) { }
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                window.ImportOverallPB.Value = 95;
                window.ImportCurrentPB.Value = 50;
                window.ImportStatusText.Content = "Building New Resource Pack - Cleanup and Includes";
            });

            if (IncludePath != "")
            {
                Copy(IncludePath, OutputPath);
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            App.Current.Dispatcher.Invoke(
                (Action<TimeSpan>)((tss) =>
                {
                    window.ShrinkOverallPB.Value = 0;
                    window.ShrinkCurrentPB.Value = 0;
                    window.ShrinkStatusText.Content = "Success! Completed in " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", tss.Hours, tss.Minutes, tss.Seconds, tss.Milliseconds / 10);
                    window.ShrinkStart.IsEnabled = true;
                }),
                new object[] { ts }
            );

            window.showMSGBox("Content shrinking was successful!\n\nYour Shrunken Content Pack is located in:\n" + OutputPath, "Content Shrink Success");
        }
    }
}
