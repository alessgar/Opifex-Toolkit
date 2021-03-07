using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace OpifexToolkit
{
    public class Importer
    {
        bool LibBuilt = false;
        Dictionary<string, List<string>> sourceModelMaps = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> sourceMaterialMaps = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> sourceTextureMaps = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> sourceSoundMaps = new Dictionary<string, List<string>>();
        Dictionary<string, int> defGFiles = new Dictionary<string, int>();
        Dictionary<string, List<string>> FoundModels = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> FoundMaterials = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> FoundTextures = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> FoundSounds = new Dictionary<string, List<string>>();
        List<int> foundGames = new List<int> { };
        string[] sourceGamesDirs = { "Black Mesa\\bms", "infra\\infra", "Counter-Strike Source\\cstrike", "Half-Life 2\\hl1", "Half-Life 2\\episodic", "Half-Life 2\\ep2", "Counter-Strike Global Offensive\\csgo", "Portal 2\\portal2", "Left 4 Dead 2\\left4dead2", "left 4 dead\\left4dead", "Team Fortress 2\\tf", "Team Fortress 2\\tf", "Portal\\portal", "Portal\\hl2", "Portal\\hl2", "Half-Life 2\\hl2", "Half-Life 2\\hl2", "GarrysMod\\garrysmod" };
        string[] sourceModelsList = { "Black Mesa\\bms\\bms_models_dir.vpk", "infra\\infra\\pak01_dir.vpk", "Counter-Strike Source\\cstrike\\cstrike_pak_dir.vpk", "Half-Life 2\\hl1\\hl1_pak_dir.vpk", "Half-Life 2\\episodic\\ep1_pak_dir.vpk", "Half-Life 2\\ep2\\ep2_pak_dir.vpk", "Counter-Strike Global Offensive\\csgo\\pak01_dir.vpk", "Portal 2\\portal2\\pak01_dir.vpk", "Left 4 Dead 2\\left4dead2\\pak01_dir.vpk", "left 4 dead\\left4dead\\pak01_dir.vpk", "Team Fortress 2\\tf\\tf2_misc_dir.vpk", "Team Fortress 2\\tf\\tf2_misc_dir.vpk", "Portal\\portal\\portal_pak_dir.vpk", "Portal\\hl2\\hl2_misc_dir.vpk", "Portal\\hl2\\hl2_misc_dir.vpk", "Half-Life 2\\hl2\\hl2_misc_dir.vpk", "Half-Life 2\\hl2\\hl2_misc_dir.vpk", "GarrysMod\\garrysmod\\garrysmod_dir.vpk" };
        string[] sourceMaterialsList = { "Black Mesa\\bms\\bms_materials_dir.vpk", "infra\\infra\\pak01_dir.vpk", "Counter-Strike Source\\cstrike\\cstrike_pak_dir.vpk", "Half-Life 2\\hl1\\hl1_pak_dir.vpk", "Half-Life 2\\episodic\\ep1_pak_dir.vpk", "Half-Life 2\\ep2\\ep2_pak_dir.vpk", "Counter-Strike Global Offensive\\csgo\\pak01_dir.vpk", "Portal 2\\portal2\\pak01_dir.vpk", "Left 4 Dead 2\\left4dead2\\pak01_dir.vpk", "left 4 dead\\left4dead\\pak01_dir.vpk", "Team Fortress 2\\tf\\tf2_misc_dir.vpk", "Team Fortress 2\\tf\\tf2_misc_dir.vpk", "Portal\\portal\\portal_pak_dir.vpk", "Portal\\hl2\\hl2_misc_dir.vpk", "Portal\\hl2\\hl2_misc_dir.vpk", "Half-Life 2\\hl2\\hl2_misc_dir.vpk", "Half-Life 2\\hl2\\hl2_misc_dir.vpk", "GarrysMod\\garrysmod\\garrysmod_dir.vpk" };
        string[] sourceTexturesList = { "Black Mesa\\bms\\bms_textures_dir.vpk", "infra\\infra\\pak01_dir.vpk", "Counter-Strike Source\\cstrike\\cstrike_pak_dir.vpk", "Half-Life 2\\hl1\\hl1_pak_dir.vpk", "Half-Life 2\\episodic\\ep1_pak_dir.vpk", "Half-Life 2\\ep2\\ep2_pak_dir.vpk", "Counter-Strike Global Offensive\\csgo\\pak01_dir.vpk", "Portal 2\\portal2\\pak01_dir.vpk", "Left 4 Dead 2\\left4dead2\\pak01_dir.vpk", "left 4 dead\\left4dead\\pak01_dir.vpk", "Team Fortress 2\\tf\\tf2_textures_dir.vpk", "Team Fortress 2\\tf\\tf2_textures_dir.vpk", "Portal\\portal\\portal_pak_dir.vpk", "Portal\\hl2\\hl2_textures_dir.vpk", "Portal\\hl2\\hl2_textures_dir.vpk", "Half-Life 2\\hl2\\hl2_textures_dir.vpk", "Half-Life 2\\hl2\\hl2_textures_dir.vpk", "GarrysMod\\garrysmod\\garrysmod_dir.vpk" };
        string[] sourceSoundsList = { "Black Mesa\\bms\\bms_sounds_misc_dir.vpk", "infra\\infra\\pak01_dir.vpk", "Counter-Strike Source\\cstrike\\cstrike_pak_dir.vpk", "Half-Life 2\\hl1\\hl1_pak_dir.vpk", "Half-Life 2\\episodic\\ep1_pak_dir.vpk", "Half-Life 2\\ep2\\ep2_pak_dir.vpk", "Counter-Strike Global Offensive\\csgo\\pak01_dir.vpk", "Portal 2\\portal2\\pak01_dir.vpk", "Left 4 Dead 2\\left4dead2\\pak01_dir.vpk", "left 4 dead\\left4dead\\pak01_dir.vpk", "Team Fortress 2\\tf\\tf2_sound_misc_dir.vpk", "Team Fortress 2\\tf\\tf2_sound_vo_english_dir.vpk", "Portal\\portal\\portal_pak_dir.vpk", "Portal\\hl2\\hl2_sound_misc_dir.vpk", "Portal\\hl2\\hl2_sound_vo_english_dir.vpk", "Half-Life 2\\hl2\\hl2_sound_misc_dir.vpk", "Half-Life 2\\hl2\\hl2_sound_vo_english_dir.vpk", "GarrysMod\\garrysmod\\garrysmod_dir.vpk" };



        /*
         * 
         * Content Pack Generator - Important Functions
         * NOTE: strint PT is VPK location from SourceMaterialMaps
         * 
         */
        public void ExtractFromVPK(string directory, string args)
        {
            Directory.CreateDirectory(directory);

            Process vpkExtract = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Directory.GetCurrentDirectory() + "\\" + "HLExtract.exe",
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };
            vpkExtract.Start();
            while (!vpkExtract.StandardOutput.EndOfStream)
            {
                Console.WriteLine(vpkExtract.StandardOutput.ReadToEnd());
            }
        }

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

        public bool shouldImportSound(
            string ResPath,
            string ResName,
            string VMFline,
            Dictionary<string, string> soundImportList
        )
        {
            //IF MATERIAL IS NOT TOOL MATERIAL, IS DOCUMENTED IN THE BUILT LIBS, AND NOT ALREADY MARKED FOR IMPORT
            if (sourceSoundMaps.ContainsKey(ResPath) && soundImportList.ContainsKey(ResPath + "/" + ResName) == false)
            {
                // make sure it's not a default gmod resource, ALWAYS prefer gmod resources if they have the same path
                if (defGFiles.ContainsKey("sound/" + VMFline.ToLower()) == false)
                {
                    return true;
                }
            }
            return false;
        }

        public bool shouldImportMaterial(
           string ResPath,
           string ResName,
           string VMFline,
           Dictionary<string, string> materialImportList
        )
        {
            //IF MATERIAL IS NOT TOOL MATERIAL, IS DOCUMENTED IN THE BUILT LIBS, AND NOT ALREADY MARKED FOR IMPORT
            if (sourceMaterialMaps.ContainsKey(ResPath.ToLower()) && materialImportList.ContainsKey(VMFline.ToLower()) == false)
            {
                // make sure it's not a default gmod resource, ALWAYS prefer gmod resources if they have the same path
                int totalLegal = 0;
                foreach(string innerPath in sourceMaterialsList)
                {
                    if (innerPath != "Half-Life 2\\hl2\\hl2_misc_dir.vpk" && innerPath != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && innerPath != "Portal\\hl2\\garrysmod_dir.vpk" && FoundMaterials.ContainsKey(innerPath))
                    {
                        Console.WriteLine(ResPath + "/" + ResName + ".vmt");
                        if (FoundMaterials[innerPath].Contains(ResPath+"/"+ResName+".vmt"))
                        {
                            totalLegal++;
                        }
                    }
                }

                if (defGFiles.ContainsKey("materials/" + VMFline.ToLower() + ".vmt") == false)
                {
                    return true;
                }
                else if (totalLegal > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool shouldImportTexture(
           string ResPath,
           string ResName,
           string VMFline,
           Dictionary<string, string> textureImportList
        )
        {
            //IF MATERIAL IS NOT TOOL MATERIAL, IS DOCUMENTED IN THE BUILT LIBS, AND NOT ALREADY MARKED FOR IMPORT
            if (sourceTextureMaps.ContainsKey(ResPath.ToLower()) && textureImportList.ContainsKey(VMFline.ToLower()) == false)
            {
                // make sure it's not a default gmod resource, ALWAYS prefer gmod resources if they have the same path
                if (defGFiles.ContainsKey("materials/" + VMFline.ToLower() + ".vtf") == false)
                {
                    return true;
                }
            }
            return false;
        }

        public bool shouldImportModel(
           string ResPath,
           string ResName,
           string VMFline,
           Dictionary<string, string> modelImportList
        )
        {
            //IF MATERIAL IS NOT TOOL MATERIAL, IS DOCUMENTED IN THE BUILT LIBS, AND NOT ALREADY MARKED FOR IMPORT
            if (sourceModelMaps.ContainsKey(ResPath.ToLower()) && modelImportList.ContainsKey(VMFline.ToLower()) == false)
            {
                // make sure it's not a default gmod resource, ALWAYS prefer gmod resources if they have the same path
                if (defGFiles.ContainsKey(VMFline.ToLower()) == false)
                {
                    return true;
                }
            }
            return false;
        }

        public Tuple<Dictionary<string, string>, Dictionary<string, string>> FindRelatedMaterials(
            string MatPath,
            Dictionary<string, string> materialImportList,
            Dictionary<string, string> textureImportList,
            string SteamLibPath,
            string OutputPath,
            string OrigResPath,
            string VPK,
            string ResName
        )
        {
            //begin by extract said resource for reading
            ExtractFromVPK(OutputPath + "\\temp\\materials\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MatPath + "\"" + " -d \"" + OutputPath + "\\temp\\materials\\" + OrigResPath + "\"");

            //read extracted file
            System.IO.StreamReader vmt = new System.IO.StreamReader(OutputPath + "\\temp\\materials\\" + OrigResPath + "\\" + ResName.Replace("/", "\\").Substring(ResName.Replace("/", "\\").LastIndexOf("\\") + 1));
            string VMTline;
            while ((VMTline = vmt.ReadLine()) != null)
            {
                VMTline = VMTline.Replace("\\", "/");
                //Console.WriteLine(VMTline);
                if (VMTline.ToLower().Contains("$basetexture") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false && VMTline.ToLower().Contains("$basetexturetransform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$bumpmap") || VMTline.ToLower().Contains("$basetexture2") && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$bumpmap2") || VMTline.ToLower().Contains("$blendmodulatetexture") || VMTline.ToLower().Contains("%tooltexture") || VMTline.ToLower().Contains("$envmapmask") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false || VMTline.ToLower().Contains("$detail") && VMTline.ToLower().Contains("$detailscale") == false && VMTline.ToLower().Contains("$detailblendfactor") == false && VMTline.ToLower().Contains("$detailblendmode") == false || VMTline.ToLower().Contains("$flowmap") || VMTline.ToLower().Contains("$flow_noise_texture") || VMTline.ToLower().Contains("$texture2") && VMTline.ToLower().Contains("$basetexture2") == false && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$phongexponenttexture") || VMTline.ToLower().Contains("$envmap") && VMTline.ToLower().Contains("$envmapmask") == false && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false)
                {
                    //texture
                    VMTline = VMTline.Replace(".vtf", "");
                    Tuple<string, string, string> resInfo = GetResourceInfo(VMTline);
                    VMTline = resInfo.Item1;
                    string OrigVTFPath = resInfo.Item2.ToLower();
                    string VTFPath = "materials/" + resInfo.Item2.ToLower();
                    string VTFName = resInfo.Item3.ToLower();

                    if (shouldImportTexture(VTFPath, VTFName, VMTline, textureImportList))
                    {
                        foreach (var VMTVPK in sourceTextureMaps[VTFPath])
                        {
                            if (VMTVPK != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && VMTVPK != "Half-Life 2\\hl2\\hl2_textures_dir.vpk" && VMTVPK != "Portal\\hl2\\hl2_textures_dir.vpk")
                            {
                                if (FoundTextures[VMTVPK].Contains("materials/" + VMTline.ToLower() + ".vtf") || FoundTextures[VMTVPK].Contains("materials/" + VMTline.ToLower() + ".vtf"))
                                {
                                    //Console.WriteLine(VMTline + "   -   " + VMTVPK);
                                    textureImportList[VMTline.ToLower()] = VMTVPK;

                                    break;
                                }
                            }
                        }
                    }
                }
                else if (VMTline.ToLower().Contains("include"))
                {
                    //material
                    //defined but not used since this doesn't appear once in the entire map
                    //cracked glass :(
                    Tuple<string, string, string> resInfo = GetResourceInfo(VMTline);
                    VMTline = resInfo.Item1.Substring(0, resInfo.Item1.ToLower().IndexOf(".vmt")).Substring(10);
                    string OrigVTFPath = resInfo.Item2.ToLower().Substring(10);
                    string VTFPath = resInfo.Item2.ToLower();
                    string VTFName = resInfo.Item3.ToLower().Substring(0, resInfo.Item3.ToLower().IndexOf(".vmt"));

                    if (shouldImportMaterial(VTFPath, VTFName, VMTline, materialImportList))
                    {
                        foreach (var VPKS in sourceMaterialMaps[VTFPath])
                        {
                            if (VPKS != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && VPKS != "Half-Life 2\\hl2\\hl2_misc_dir.vpk" && VPKS != "Portal\\hl2\\hl2_misc_dir.vpk")
                            {
                                if (FoundMaterials[VPKS].Contains("materials/" + VMTline.ToLower() + ".vmt") || FoundMaterials[VPKS].Contains("materials/" + VMTline.ToLower() + ".vmt"))
                                {
                                    //Console.WriteLine(VMFline + "   -   " + VPK);
                                    materialImportList[VMTline.ToLower()] = VPKS;

                                    //recursion
                                    Tuple<Dictionary<string, string>, Dictionary<string, string>> relMatRes = FindRelatedMaterials("materials/" + VMTline.ToLower() + ".vmt", materialImportList, textureImportList, SteamLibPath, OutputPath, OrigResPath, VPKS, VMTline.ToLower() + ".vmt");
                                    materialImportList = relMatRes.Item1;
                                    textureImportList = relMatRes.Item2;

                                    break;
                                }
                            }
                        }
                    }
                }
                else if (VMTline.ToLower().Contains("$bottommaterial"))
                {
                    Tuple<string, string, string> resInfo = GetResourceInfo(VMTline);
                    VMTline = resInfo.Item1;
                    string OrigVTFPath = resInfo.Item2.ToLower();
                    string VTFPath = "materials/" + resInfo.Item2.ToLower();
                    string VTFName = resInfo.Item3.ToLower();

                    if (shouldImportMaterial(VTFPath, VTFName, VMTline, materialImportList))
                    {
                        foreach (var VPKS in sourceMaterialMaps[VTFPath])
                        {
                            if (VPKS != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && VPKS != "Half-Life 2\\hl2\\hl2_misc_dir.vpk" && VPKS != "Portal\\hl2\\hl2_misc_dir.vpk")
                            {
                                if (FoundMaterials[VPKS].Contains("materials/" + VMTline.ToLower() + ".vmt") || FoundMaterials[VPKS].Contains("materials/" + VMTline.ToLower() + ".vmt"))
                                {
                                    //Console.WriteLine(VMFline + "   -   " + VPK);
                                    materialImportList[VMTline.ToLower()] = VPKS;

                                    //recursion
                                    Tuple<Dictionary<string, string>, Dictionary<string, string>> relMatRes = FindRelatedMaterials("materials/" + VMTline.ToLower() + ".vmt", materialImportList, textureImportList, SteamLibPath, OutputPath, OrigResPath, VPKS, VMTline.ToLower() + ".vmt");
                                    materialImportList = relMatRes.Item1;
                                    textureImportList = relMatRes.Item2;

                                    break;
                                }
                            }
                        }
                    }
                }
                else if (VMTline.ToLower().Contains("$crackmaterial"))
                {
                    Tuple<string, string, string> resInfo = GetResourceInfo(VMTline);
                    VMTline = resInfo.Item1;
                    string OrigVTFPath = resInfo.Item2.ToLower();
                    string VTFPath = "materials/" + resInfo.Item2.ToLower();
                    string VTFName = resInfo.Item3.ToLower();

                    //Here so that I remember one day to test this using the CSS glass
                    //If you don't use the CSS breaking glass this will never apply to you

                    /*if (shouldImportMaterial(VTFPath, VTFName, VMTline, materialImportList))
                    {
                        foreach (var VPKS in sourceMaterialMaps[VTFPath])
                        {
                            if (VPKS != "GarrysMod\\garrysmod\\garrysmod_dir.vpk")
                            {
                                if (FoundMaterials[VPKS].Contains("materials/" + VMTline.ToLower() + ".vmt") || FoundMaterials[VPKS].Contains("materials/" + VMTline.ToLower() + ".vmt"))
                                {
                                    //Console.WriteLine(VMFline + "   -   " + VPK);
                                    materialImportList[VMTline.ToLower()] = VPKS;

                                    //recursion
                                    Tuple<Dictionary<string, string>, Dictionary<string, string>> relMatRes = FindRelatedMaterials("materials/" + VMTline.ToLower() + ".vmt", materialImportList, textureImportList, SteamLibPath, OutputPath, OrigResPath, VPKS);
                                    materialImportList = relMatRes.Item1;
                                    textureImportList = relMatRes.Item2;

                                    break;
                                }
                            }
                        }
                    }
                    */
                }
            }
            vmt.Close();

            return Tuple.Create(materialImportList, textureImportList);
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

        public void RecompileModel(string GmodDir, string args, int recursion = 0)
        {
            bool working = true;

            Process vpkDecomp = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GmodDir + "\\GarrysMod\\bin\\studiomdl.exe",
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
                string line = vpkDecomp.StandardOutput.ReadToEnd();
                Console.WriteLine(line);
                if (line.Contains("EXCEPTION_ACCESS_VIOLATION"))
                {
                    working = false;
                }
            }

            if (working == false && recursion < 10)
            {
                RecompileModel(GmodDir, args, recursion + 1);
            }
        }

        public Tuple<Dictionary<string, string>, Dictionary<string, string>> FindModelMaterials(
           string MDLPath,
           Dictionary<string, string> materialImportList,
           Dictionary<string, string> textureImportList,
           string SteamLibPath,
           string OutputPath,
           string OrigResPath,
           string VPK
        )
        {
            //begin by extract and decompiling said resource for reading
            if (OrigResPath == "")
            {
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath + "\"" + " -d \"" + OutputPath + "\\temp\\models" + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".vvd" + "\"" + " -d \"" + OutputPath + "\\temp\\models" + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".sw.vvd" + "\"" + " -d \"" + OutputPath + "\\temp\\models" + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".dx90.vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models" + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".dx80.vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models" + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models" + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".sw.vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models" + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".xbox.vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models" + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".phy" + "\"" + " -d \"" + OutputPath + "\\temp\\models" + "\"");
                DecompileModel(OutputPath + "\\temp\\models\\" + OrigResPath, "-p " + "\"" + OutputPath + "\\temp\\" + MDLPath + "\"" + " -o \"" + OutputPath + "\\temp\\models" + "\"");
            }
            else
            {
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath + "\"" + " -d \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".vvd" + "\"" + " -d \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".sw.vvd" + "\"" + " -d \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".dx90.vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".dx80.vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".sw.vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".xbox.vtx" + "\"" + " -d \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
                ExtractFromVPK(OutputPath + "\\temp\\models\\" + OrigResPath, "-p \"" + SteamLibPath + "\\" + VPK + "\" -e " + "\"" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".phy" + "\"" + " -d \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
                DecompileModel(OutputPath + "\\temp\\models\\" + OrigResPath, "-p " + "\"" + OutputPath + "\\temp\\" + MDLPath + "\"" + " -o \"" + OutputPath + "\\temp\\models\\" + OrigResPath + "\"");
            }

            //read decompiled model
            System.IO.StreamReader mdl = new System.IO.StreamReader(OutputPath + "\\temp\\" + MDLPath.Substring(0, MDLPath.LastIndexOf(".mdl")) + ".qc");
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
                    if (MDLline.Substring(MDLline.IndexOf("\"") + 1, MDLline.LastIndexOf("\"") - MDLline.IndexOf("\"")).Replace("\\", "/").ToLower().EndsWith("/\"")){
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
                                string VMFline = OrigMatPath + "/" + MatName;

                                if (shouldImportMaterial(MatPath, MatName, VMFline, materialImportList))
                                {
                                    foreach (var MATVPK in sourceMaterialMaps[MatPath])
                                    {
                                        if (MATVPK != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && MATVPK != "Half-Life 2\\hl2\\hl2_misc_dir.vpk" && MATVPK != "Portal\\hl2\\hl2_misc_dir.vpk")
                                        {
                                            if (FoundMaterials[MATVPK].Contains("materials/" + VMFline.ToLower() + ".vmt") || FoundMaterials[MATVPK].Contains("materials/" + VMFline.ToLower() + ".vmt"))
                                            {
                                                //Console.WriteLine(VMFline + "   -   " + VPK);
                                                materialImportList[VMFline.ToLower()] = MATVPK;

                                                Console.WriteLine(VMFline.ToLower());

                                                //recursion
                                                Tuple<Dictionary<string, string>, Dictionary<string, string>> relMatRes = FindRelatedMaterials("materials/" + VMFline.ToLower() + ".vmt", materialImportList, textureImportList, SteamLibPath, OutputPath, OrigMatPath, MATVPK, VMFline.ToLower() + ".vmt");
                                                materialImportList = relMatRes.Item1;
                                                textureImportList = relMatRes.Item2;

                                                break;
                                            }
                                        }
                                    }
                                }
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
                //no skins, find and use default
                foreach (string curMaterialPath in curMaterialPaths)
                {
                    Console.WriteLine(curMaterialPath);
                    string OrigMatPath = curMaterialPath.ToLower();
                    string MatPath = "materials/" + OrigMatPath;
                    string MatName = MDLPath.Substring(MDLPath.LastIndexOf("/") + 1, MDLPath.LastIndexOf(".mdl") - MDLPath.LastIndexOf("/") - 1);
                    Console.WriteLine(MatName);
                    string VMFline = OrigMatPath + "/" + MatName;

                    if (shouldImportMaterial(MatPath, MatName, VMFline, materialImportList))
                    {
                        foreach (var MATVPK in sourceMaterialMaps[MatPath])
                        {
                            if (MATVPK != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && MATVPK != "Half-Life 2\\hl2\\hl2_misc_dir.vpk" && MATVPK != "Portal\\hl2\\hl2_misc_dir.vpk")
                            {
                                if (FoundMaterials[MATVPK].Contains("materials/" + VMFline.ToLower() + ".vmt") || FoundMaterials[MATVPK].Contains("materials/" + VMFline.ToLower() + ".vmt"))
                                {
                                    //Console.WriteLine(VMFline + "   -   " + VPK);
                                    materialImportList[VMFline.ToLower()] = MATVPK;

                                    Console.WriteLine(VMFline.ToLower());

                                    //recursion
                                    Tuple<Dictionary<string, string>, Dictionary<string, string>> relMatRes = FindRelatedMaterials("materials/" + VMFline.ToLower() + ".vmt", materialImportList, textureImportList, SteamLibPath, OutputPath, OrigMatPath, MATVPK, VMFline.ToLower() + ".vmt");
                                    materialImportList = relMatRes.Item1;
                                    textureImportList = relMatRes.Item2;

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return Tuple.Create(materialImportList, textureImportList);
        }

        public Tuple<Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>> VMFFirstRunHandler(
        string VMFline,
        Dictionary<string, string> soundImportList,
        Dictionary<string, string> materialImportList,
        Dictionary<string, string> textureImportList,
        Dictionary<string, string> modelImportList,
        string SteamLibPath,
        string OutputPath
    )
        {
            string txtLine = VMFline;

            //Console.WriteLine(VMFline);

            if (VMFline.ToLower().Contains(".mp3") || VMFline.ToLower().Contains(".wav"))
            {
                //sound handler (1st run)
                Tuple<string, string, string> resInfo = GetResourceInfo(VMFline);
                VMFline = resInfo.Item1;
                string OrigResPath = resInfo.Item2.ToLower();
                string ResPath = "sound/" + resInfo.Item2.ToLower();
                string ResName = resInfo.Item3.ToLower();

                if (shouldImportSound(ResPath, ResName, VMFline, soundImportList))
                {
                    foreach (var VPK in sourceSoundMaps[ResPath])
                    {
                        if (VPK != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && VPK != "Half-Life 2\\hl2\\hl2_sound_misc_dir.vpk" && VPK != "Half-Life 2\\hl2\\hl2_sound_vo_english_dir.vpk" && VPK != "Portal\\hl2\\hl2_sound_misc_dir.vpk" && VPK != "Portal\\hl2\\hl2_sound_vo_english_dir.vpk")
                        {
                            //Console.WriteLine("sound/" + VMFline.ToLower());
                            if (FoundSounds[VPK].Contains("sound/" + VMFline.ToLower()) || FoundSounds[VPK].Contains("sound/" + VMFline.ToLower()))
                            {
                                //Console.WriteLine(VMFline + "   -   " + VPK);
                                soundImportList[VMFline] = VPK;
                                break;
                            }
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

                if (shouldImportMaterial(ResPath, ResName, VMFline, materialImportList))
                {
                    foreach (var VPK in sourceMaterialMaps[ResPath])
                    {
                        if (VPK != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && VPK != "Half-Life 2\\hl2\\hl2_misc_dir.vpk" && VPK != "Portal\\hl2\\hl2_misc_dir.vpk")
                        {
                            if (FoundMaterials[VPK].Contains("materials/" + VMFline.ToLower() + ".vmt") || FoundMaterials[VPK].Contains("materials/" + VMFline.ToLower() + ".vmt"))
                            {
                                //Console.WriteLine(VMFline + "   -   " + VPK);
                                materialImportList[VMFline.ToLower()] = VPK;

                                //recursion
                                Tuple<Dictionary<string, string>, Dictionary<string, string>> relMatRes = FindRelatedMaterials("materials/" + VMFline.ToLower() + ".vmt", materialImportList, textureImportList, SteamLibPath, OutputPath, OrigResPath, VPK, VMFline.ToLower() + ".vmt");
                                materialImportList = relMatRes.Item1;
                                textureImportList = relMatRes.Item2;

                                break;
                            }
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

                if (shouldImportModel(ResPath, ResName, VMFline, modelImportList))
                {
                    foreach (var VPK in sourceModelMaps[ResPath])
                    {
                        if (VPK != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && VPK != "Half-Life 2\\hl2\\hl2_misc_dir.vpk" && VPK != "Portal\\hl2\\hl2_misc_dir.vpk")
                        {
                            if (FoundModels[VPK].Contains(VMFline.ToLower()) || FoundModels[VPK].Contains(VMFline.ToLower()))
                            {
                                //Console.WriteLine(VMFline + "   -   " + VPK);
                                modelImportList[VMFline.ToLower()] = VPK;

                                //recursion
                                Tuple<Dictionary<string, string>, Dictionary<string, string>> relMatRes = FindModelMaterials(VMFline.ToLower(), materialImportList, textureImportList, SteamLibPath, OutputPath, OrigResPath, VPK);
                                materialImportList = relMatRes.Item1;
                                textureImportList = relMatRes.Item2;

                                break;
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

                if (shouldImportMaterial(ResPath, ResName, VMFline, materialImportList))
                {
                    foreach (var VPK in sourceMaterialMaps[ResPath])
                    {
                        if (VPK != "GarrysMod\\garrysmod\\garrysmod_dir.vpk" && VPK != "Half-Life 2\\hl2\\hl2_misc_dir.vpk" && VPK != "Portal\\hl2\\hl2_misc_dir.vpk")
                        {
                            if (FoundMaterials[VPK].Contains("materials/" + VMFline.ToLower() + ".vmt") || FoundMaterials[VPK].Contains("materials/" + VMFline.ToLower() + ".vmt"))
                            {
                                //Console.WriteLine(VMFline + "   -   " + VPK);
                                materialImportList[VMFline.ToLower()] = VPK;

                                //recursion
                                Tuple<Dictionary<string, string>, Dictionary<string, string>> relMatRes = FindRelatedMaterials("materials/" + VMFline.ToLower() + ".vmt", materialImportList, textureImportList, SteamLibPath, OutputPath, OrigResPath, VPK, VMFline.ToLower() + ".vmt");
                                materialImportList = relMatRes.Item1;
                                textureImportList = relMatRes.Item2;

                                break;
                            }
                        }
                    }
                }
            }

            return Tuple.Create(soundImportList, materialImportList, textureImportList, modelImportList);
        }

        public void BuildLibraryList(
            string SteamLibPath,
            MainWindow window
        )
        {
            if (LibBuilt == false)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    window.ImportStatusText.Content = "Building Game Content Mappings";
                });

                //find games we have in our library
                int i = 0;
                foreach (string s in sourceGamesDirs)
                {
                    string dir = SteamLibPath + "\\" + s;
                    if (Directory.Exists(dir))
                    {
                        foundGames.Add(i);
                    }
                    i++;
                }

                //make sure the library has stuff in it
                if (foundGames.Count() > 0)
                {
                    string gmodReqDir = SteamLibPath + "\\GarrysMod\\bin";
                    if (Directory.Exists(gmodReqDir))
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            window.ImportStatusText.Content = "Building Game Content Mappings";
                        });

                        int GameCounter = 1;

                        foreach (int a in foundGames)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                window.ImportCurrentPB.Value = 0;
                            });

                            List<float> sBar = new List<float>();
                            sBar.Add((float)GameCounter);
                            sBar.Add((float)foundGames.Count());
                            App.Current.Dispatcher.Invoke(
                                (Action<List<float>>)((VMFLine) =>
                                {
                                    window.ImportStatusText.Content = "Building Game Content Mappings (" + VMFLine[0] + "/" + VMFLine[1] + ")";
                                }),
                                new object[] { sBar }
                            );

                            //Model Maps
                            var vpkScan = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = gmodReqDir + "\\" + "vpk.exe",
                                    Arguments = "l \"" + SteamLibPath + "\\" + sourceModelsList[a] + "\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                }
                            };
                            vpkScan.Start();
                            string output = "";
                            while (!vpkScan.StandardOutput.EndOfStream)
                            {
                                output = vpkScan.StandardOutput.ReadToEnd();
                            }
                            string[] conOutModel = output.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (string line in conOutModel)
                            {
                                if (line.StartsWith("models/"))
                                {
                                    //Console.WriteLine(line);
                                    int subEnd = line.LastIndexOf(@"/");
                                    if (subEnd != -1)
                                    {
                                        string dicPath = line.Substring(0, subEnd);
                                        if (sourceModelMaps.ContainsKey(dicPath) == false)
                                        {
                                            sourceModelMaps[dicPath] = new List<string>();
                                        }
                                        if (sourceModelMaps[dicPath].Contains(sourceModelsList[a]) == false)
                                        {
                                            sourceModelMaps[dicPath].Add(sourceModelsList[a]);
                                        }
                                        if (sourceModelsList[a] == "GarrysMod\\garrysmod\\garrysmod_dir.vpk" || sourceModelsList[a] == "Half-Life 2\\hl2\\hl2_misc_dir.vpk" || sourceModelsList[a] == "Portal\\hl2\\hl2_misc_dir.vpk")
                                        {
                                            string fName = line;
                                            defGFiles[fName] = 1;
                                        }
                                        else
                                        {
                                            string fName = line;
                                            if (FoundModels.ContainsKey(sourceModelsList[a]) == false)
                                            {
                                                FoundModels[sourceModelsList[a]] = new List<string>();
                                            }
                                            if (FoundModels[sourceModelsList[a]].Contains(fName) == false)
                                            {
                                                FoundModels[sourceModelsList[a]].Add(fName);
                                            }
                                        }
                                    }
                                }
                            }

                            App.Current.Dispatcher.Invoke(() =>
                            {
                                window.ImportCurrentPB.Value = 25;
                            });

                            //Material Maps
                            vpkScan = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = gmodReqDir + "\\" + "vpk.exe",
                                    Arguments = "l \"" + SteamLibPath + "\\" + sourceMaterialsList[a] + "\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                }
                            };
                            vpkScan.Start();
                            output = "";
                            while (!vpkScan.StandardOutput.EndOfStream)
                            {
                                output = vpkScan.StandardOutput.ReadToEnd();
                            }
                            string[] conOutMaterial = output.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (string line in conOutMaterial)
                            {
                                if (line.StartsWith("materials/") && line.EndsWith(".vmt"))
                                {
                                    //Console.WriteLine(line);
                                    int subEnd = line.LastIndexOf(@"/");
                                    if (subEnd != -1)
                                    {
                                        string dicPath = line.Substring(0, subEnd);
                                        if (sourceMaterialMaps.ContainsKey(dicPath) == false)
                                        {
                                            sourceMaterialMaps[dicPath] = new List<string>();
                                        }
                                        if (sourceMaterialMaps[dicPath].Contains(sourceMaterialsList[a]) == false)
                                        {
                                            sourceMaterialMaps[dicPath].Add(sourceMaterialsList[a]);
                                        }
                                        if (sourceMaterialsList[a] == "GarrysMod\\garrysmod\\garrysmod_dir.vpk" || sourceMaterialsList[a] == "Half-Life 2\\hl2\\hl2_misc_dir.vpk" || sourceMaterialsList[a] == "Portal\\hl2\\hl2_misc_dir.vpk")
                                        {
                                            string fName = line;
                                            defGFiles[fName] = 1;
                                        }
                                        else
                                        {
                                            string fName = line;
                                            if (FoundMaterials.ContainsKey(sourceMaterialsList[a]) == false)
                                            {
                                                FoundMaterials[sourceMaterialsList[a]] = new List<string>();
                                            }
                                            if (FoundMaterials[sourceMaterialsList[a]].Contains(fName) == false)
                                            {
                                                FoundMaterials[sourceMaterialsList[a]].Add(fName);
                                            }
                                        }
                                    }
                                }
                            }


                            App.Current.Dispatcher.Invoke(() =>
                            {
                                window.ImportCurrentPB.Value = 50;
                            });





                            //Texture Maps
                            vpkScan = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = gmodReqDir + "\\" + "vpk.exe",
                                    Arguments = "l \"" + SteamLibPath + "\\" + sourceTexturesList[a] + "\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                }
                            };
                            vpkScan.Start();
                            output = "";
                            while (!vpkScan.StandardOutput.EndOfStream)
                            {
                                output = vpkScan.StandardOutput.ReadToEnd();
                            }
                            string[] conOutTexture = output.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (string line in conOutTexture)
                            {
                                if (line.StartsWith("materials/") && line.EndsWith(".vtf"))
                                {
                                    //Console.WriteLine(line);
                                    int subEnd = line.LastIndexOf(@"/");
                                    if (subEnd != -1)
                                    {
                                        string dicPath = line.Substring(0, subEnd);
                                        if (sourceTextureMaps.ContainsKey(dicPath) == false)
                                        {
                                            sourceTextureMaps[dicPath] = new List<string>();
                                        }
                                        if (sourceTextureMaps[dicPath].Contains(sourceTexturesList[a]) == false)
                                        {
                                            sourceTextureMaps[dicPath].Add(sourceTexturesList[a]);
                                        }
                                        if (sourceTexturesList[a] == "GarrysMod\\garrysmod\\garrysmod_dir.vpk" || sourceTexturesList[a] == "Half-Life 2\\hl2\\hl2_textures_dir.vpk" || sourceTexturesList[a] == "Portal\\hl2\\hl2_textures_dir.vpk")
                                        {
                                            string fName = line;
                                            defGFiles[fName] = 1;
                                        }
                                        else
                                        {
                                            string fName = line;
                                            if (FoundTextures.ContainsKey(sourceTexturesList[a]) == false)
                                            {
                                                FoundTextures[sourceTexturesList[a]] = new List<string>();
                                            }
                                            if (FoundTextures[sourceTexturesList[a]].Contains(fName) == false)
                                            {
                                                FoundTextures[sourceTexturesList[a]].Add(fName);
                                            }
                                        }
                                    }
                                }
                            }

                            App.Current.Dispatcher.Invoke(() =>
                            {
                                window.ImportCurrentPB.Value = 75;
                            });





                            //Sound Maps
                            vpkScan = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = gmodReqDir + "\\" + "vpk.exe",
                                    Arguments = "l \"" + SteamLibPath + "\\" + sourceSoundsList[a] + "\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                }
                            };
                            vpkScan.Start();
                            output = "";
                            while (!vpkScan.StandardOutput.EndOfStream)
                            {
                                output = vpkScan.StandardOutput.ReadToEnd();
                            }
                            string[] conOutSound = output.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (string line in conOutSound)
                            {
                                if (line.StartsWith("sound/"))
                                {
                                    //Console.WriteLine(line);
                                    int subEnd = line.LastIndexOf(@"/");
                                    if (subEnd != -1)
                                    {
                                        string dicPath = line.Substring(0, subEnd);
                                        if (sourceSoundMaps.ContainsKey(dicPath) == false)
                                        {
                                            sourceSoundMaps[dicPath] = new List<string>();
                                        }
                                        if (sourceSoundMaps[dicPath].Contains(sourceSoundsList[a]) == false)
                                        {
                                            sourceSoundMaps[dicPath].Add(sourceSoundsList[a]);
                                        }
                                        if (sourceSoundsList[a] == "GarrysMod\\garrysmod\\garrysmod_dir.vpk" || sourceSoundsList[a] == "Half-Life 2\\hl2\\hl2_sound_misc_dir.vpk" || sourceSoundsList[a] == "Half-Life 2\\hl2\\hl2_sound_vo_english_dir.vpk" || sourceSoundsList[a] == "Portal\\hl2\\hl2_sound_misc_dir.vpk" || sourceSoundsList[a] == "Portal\\hl2\\hl2_sound_vo_english_dir.vpk")
                                        {
                                            string fName = line;
                                            defGFiles[fName] = 1;
                                        }
                                        else
                                        {
                                            string fName = line;
                                            if (FoundSounds.ContainsKey(sourceSoundsList[a]) == false)
                                            {
                                                FoundSounds[sourceSoundsList[a]] = new List<string>();
                                            }
                                            if (FoundSounds[sourceSoundsList[a]].Contains(fName) == false)
                                            {
                                                FoundSounds[sourceSoundsList[a]].Add(fName);
                                            }
                                        }
                                    }
                                }
                            }





                            App.Current.Dispatcher.Invoke(
                                (Action<int, List<int>>)((GameCounte, foundGame) =>
                                {
                                    window.ImportCurrentPB.Value = 100;
                                    int GC = GameCounte;
                                    int FG = foundGame.Count();
                                    double CB = (double)GC / FG;
                                    window.ImportOverallPB.Value = 10 * CB;
                                }),
                                new object[] { GameCounter, foundGames }
                            );

                            GameCounter++;
                        }
                    }
                }

                LibBuilt = true;
            }
        }

        public void VMFSecondRunHandler(
           Dictionary<string, string> soundImportList,
           Dictionary<string, string> materialImportList,
           Dictionary<string, string> textureImportList,
           Dictionary<string, string> modelImportList,
           string SteamLibPath,
           string OutputPath,
           string ContentID,
           bool MSLOD,
           List<string> MSLODF,
           MainWindow window
        )
        {
            int textureCount = textureImportList.Count();
            int soundCount = soundImportList.Count();
            int materialCount = materialImportList.Count();
            int modelCount = modelImportList.Count();

            int currentVal = 0;
            //extract textures (no editing required) (does not contain file type)
            foreach (KeyValuePair<string, string> entry in textureImportList)
            {
                if (!ContentID.Equals(""))
                {
                    ExtractFromVPK(OutputPath + "\\materials\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "materials/" + entry.Key + ".vtf" + "\"" + " -d \"" + OutputPath + "\\materials\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")) + "\"");
                }
                else
                {
                    ExtractFromVPK(OutputPath + "\\materials\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "materials/" + entry.Key + ".vtf" + "\"" + " -d \"" + OutputPath + "\\materials\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")) + "\"");
                }

                currentVal++;
                App.Current.Dispatcher.Invoke(
                    (Action<int, int, string>)((VMFLineCounte, VMFLine, fileName) =>
                    {
                        window.ImportStatusText.Content = "Porting Textures (Step 2/3) (" + VMFLineCounte.ToString() + "/" + VMFLine.ToString() + ") - " + fileName;
                        double CB = (double)VMFLineCounte / VMFLine;
                        window.ImportCurrentPB.Value = CB * (double)100;
                        window.ImportOverallPB.Value = (CB * (double)12.5) + 40;
                    }),
                    new object[] { currentVal, textureCount, "materials/" + entry.Key + ".vtf" }
                );
            }

            currentVal = 0;
            //extract sounds (not editing required) (also contains file type)
            foreach (KeyValuePair<string, string> entry in soundImportList)
            {
                if (!ContentID.Equals(""))
                {
                    ExtractFromVPK(OutputPath + "\\sound\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "sound/" + entry.Key + "\"" + " -d \"" + OutputPath + "\\sound\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")) + "\"");
                }
                else
                {
                    ExtractFromVPK(OutputPath + "\\sound\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "sound/" + entry.Key + "\"" + " -d \"" + OutputPath + "\\sound\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")) + "\"");
                }

                currentVal++;
                App.Current.Dispatcher.Invoke(
                    (Action<int, int, string>)((VMFLineCounte, VMFLine, fileName) =>
                    {
                        window.ImportStatusText.Content = "Porting Sounds (Step 2/3) (" + VMFLineCounte.ToString() + "/" + VMFLine.ToString() + ") - " + fileName;
                        double CB = (double)VMFLineCounte / VMFLine;
                        window.ImportCurrentPB.Value = CB * (double)100;
                        window.ImportOverallPB.Value = (CB * (double)12.5) + 52.5;
                    }),
                    new object[] { currentVal, soundCount, "sound/" + entry.Key }
                );
            }

            currentVal = 0;
            //extract materials (down the rabbit hole we go)
            foreach (KeyValuePair<string, string> entry in materialImportList)
            {
                if (!ContentID.Equals(""))
                {
                    ExtractFromVPK(OutputPath + "\\materials\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "materials/" + entry.Key + ".vmt" + "\"" + " -d \"" + OutputPath + "\\materials\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")) + "\"");
                }
                else
                {
                    ExtractFromVPK(OutputPath + "\\materials\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "materials/" + entry.Key + ".vmt" + "\"" + " -d \"" + OutputPath + "\\materials\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")) + "\"");
                }

                List<string> newFile = new List<string>();
                System.IO.StreamReader vmt = new System.IO.StreamReader(OutputPath + "\\materials\\" + ContentID + "\\" + entry.Key + ".vmt");
                string VMTline;
                while ((VMTline = vmt.ReadLine()) != null)
                {
                    VMTline = VMTline.Replace("\\", "/");
                    if (VMTline.ToLower().Contains("$basetexture") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false && VMTline.ToLower().Contains("$basetexturetransform") == false || VMTline.ToLower().Contains("$bumpmap") || VMTline.ToLower().Contains("$basetexture2") && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$bumpmap2") || VMTline.ToLower().Contains("$blendmodulatetexture") || VMTline.ToLower().Contains("%tooltexture") || VMTline.ToLower().Contains("$envmapmask") && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false || VMTline.ToLower().Contains("$detail") && VMTline.ToLower().Contains("$detailscale") == false && VMTline.ToLower().Contains("$detailblendfactor") == false && VMTline.ToLower().Contains("$detailblendmode") == false || VMTline.ToLower().Contains("$flowmap") || VMTline.ToLower().Contains("$flow_noise_texture") || VMTline.ToLower().Contains("$texture2") && VMTline.ToLower().Contains("$basetexture2") == false && VMTline.ToLower().Contains("$texture2transform") == false && VMTline.ToLower().Contains("animatedtexturevar") == false && VMTline.ToLower().Contains("animatedtextureframenumvar") == false || VMTline.ToLower().Contains("$phongexponenttexture") || VMTline.ToLower().Contains("$envmap") && VMTline.ToLower().Contains("$envmapmask") == false && VMTline.ToLower().Contains("$basetexturealphaenvmapmask") == false) 
                    {
                        //texture
                        VMTline = VMTline.Replace(".vtf", "");
                        Tuple<string, string, string> resInfo = GetResourceInfo(VMTline);
                        string TVMTline = resInfo.Item1;
                        if (textureImportList.ContainsKey(TVMTline.ToLower()))
                        {
                            if (!ContentID.Equals(""))
                            {
                                VMTline = VMTline.Replace(resInfo.Item1, ContentID + "/" + resInfo.Item1);
                            }
                        }
                    }
                    else if (VMTline.ToLower().Contains("include"))
                    {
                        //material
                        Tuple<string, string, string> resInfo = GetResourceInfo(VMTline);
                        string TVMTline = resInfo.Item1.Substring(0, resInfo.Item1.ToLower().IndexOf(".vmt")).Substring(10);
                        if (materialImportList.ContainsKey(TVMTline.ToLower())){
                            if (!ContentID.Equals(""))
                            {
                                VMTline = VMTline.Replace(resInfo.Item1, "materials/" + ContentID + "/" + resInfo.Item1.Substring(10));
                            }
                            else
                            {
                                VMTline = VMTline.Replace(resInfo.Item1, "materials/" + resInfo.Item1.Substring(10));
                            }
                        }
                        Console.WriteLine(VMTline);
                    }
                    else if (VMTline.ToLower().Contains("$bottommaterial"))
                    {
                        Tuple<string, string, string> resInfo = GetResourceInfo(VMTline);
                        string TVMTline = resInfo.Item1;
                        if (materialImportList.ContainsKey(TVMTline.ToLower()))
                        {
                            if (!ContentID.Equals(""))
                            {
                                VMTline = VMTline.Replace(resInfo.Item1, ContentID + "/" + resInfo.Item1);
                            }
                        }
                    }
                    else if (VMTline.ToLower().Contains("$crackmaterial"))
                    {
                        //Only useful for maps with CSS glass, I will finish this handler at some point or if somebody asks nicely
                    }

                    newFile.Add(VMTline);
                }
                vmt.Close();

                System.IO.StreamWriter file;
                if (!ContentID.Equals(""))
                {
                    using (file =
                    new System.IO.StreamWriter(OutputPath + "\\materials\\" + ContentID + "\\" + entry.Key + ".vmt", false))
                    {
                        foreach (string line in newFile)
                        {
                            file.WriteLine(line);
                        }
                    }
                }
                else
                {
                    using (file =
                    new System.IO.StreamWriter(OutputPath + "\\materials\\" + entry.Key + ".vmt", false))
                    {
                        foreach (string line in newFile)
                        {
                            file.WriteLine(line);
                        }
                    }
                }

                currentVal++;
                App.Current.Dispatcher.Invoke(
                    (Action<int, int, string>)((VMFLineCounte, VMFLine, fileName) =>
                    {
                        window.ImportStatusText.Content = "Porting Materials (Step 2/3) (" + VMFLineCounte.ToString() + "/" + VMFLine.ToString() + ") - " + fileName;
                        double CB = (double)VMFLineCounte / VMFLine;
                        window.ImportCurrentPB.Value = CB * (double)100;
                        window.ImportOverallPB.Value = (CB * (double)12.5) + 65;
                    }),
                    new object[] { currentVal, materialCount, "materials/" + entry.Key + ".vmt" }
                );
            }

            //copy all sound, textures, and materials to GMOD respective folders. StudioMDL will take care of this for models automatically
            if (Directory.Exists(OutputPath + "\\materials\\"))
            {
                DirectoryCopy(OutputPath + "\\materials\\", SteamLibPath + "\\GarrysMod\\garrysmod\\materials\\", true);
            }

            if (Directory.Exists(OutputPath + "\\sound\\"))
            {
                DirectoryCopy(OutputPath + "\\sound\\", SteamLibPath + "\\GarrysMod\\garrysmod\\sound\\", true);
            }

            currentVal = 0;
            foreach (KeyValuePair<string, string> entry in modelImportList)
            {
                //begin by extract and decompiling said resource for reading
                Console.WriteLine(entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""));
                if (entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") == "models")
                {
                    if (!ContentID.Equals(""))
                    {
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Replace("models/", "") + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".xbox.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\"");
                        DecompileModel(OutputPath + "\\models\\" + ContentID + "\\", "-p " + "\"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Replace("models/", "") + "\"" + " -o \"" + OutputPath + "\\models\\" + ContentID + "\"");
                    }
                    else
                    {
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Replace("models/", "") + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg" + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd" + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd" + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx" + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx" + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vtx" + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx" + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".xbox.vtx" + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\", "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy" + "\"" + " -d \"" + OutputPath + "\\models" + "\"");
                        DecompileModel(OutputPath + "\\models\\", "-p " + "\"" + OutputPath + "\\models\\" + entry.Key.Replace("models/", "") + "\"" + " -o \"" + OutputPath + "\\models" + "\"");
                    }
                }
                else
                {
                    if (!ContentID.Equals(""))
                    {
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Replace("models/", "") + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".xbox.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy" + "\"" + " -d \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        DecompileModel(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p " + "\"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Replace("models/", "") + "\"" + " -o \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                    }
                    else
                    {
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Replace("models/", "") + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg" + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd" + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd" + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".xbox.vtx" + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        ExtractFromVPK(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p \"" + SteamLibPath + "\\" + entry.Value + "\" -e " + "\"" + "models/" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy" + "\"" + " -d \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                        DecompileModel(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", ""), "-p " + "\"" + OutputPath + "\\models\\" + entry.Key.Replace("models/", "") + "\"" + " -o \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\"");
                    }
                }

                List<string> newFile = new List<string>();
                System.IO.StreamReader vmt;
                if (!ContentID.Equals(""))
                {
                    vmt = new System.IO.StreamReader(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".qc");
                }
                else
                {
                    vmt = new System.IO.StreamReader(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".qc");
                }
                string VMTline;
                bool sequenceSet = false;
                int sequenceLevel = 0;
                bool ignoreSeq = true;
                while ((VMTline = vmt.ReadLine()) != null)
                {
                    VMTline = VMTline.Replace("\\", "/");
                    if (VMTline.ToLower().Contains("$modelname"))
                    {
                        if (!ContentID.Equals(""))
                        {
                            VMTline = VMTline.Replace(VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 2), ContentID + "/" + VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 2));
                        }
                    }
                    else if (VMTline.ToLower().Contains("$cdmaterials") && !VMTline.Contains("$cdmaterials \"\""))
                    {
                        Console.WriteLine(VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"")).Replace("\\", "/").ToLower());
                        if (VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"")).Replace("\\", "/").ToLower().EndsWith("/\""))
                        {
                            if (!ContentID.Equals(""))
                            {
                                VMTline = VMTline.Replace(VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 2), ContentID + "/" + VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 2));
                            }
                        }
                        else if (VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"")).Replace("\\", "/").ToLower().EndsWith("\""))
                        {
                            if (!ContentID.Equals(""))
                            {
                                VMTline = VMTline.Replace(VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 1), ContentID + "/" + VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 1));
                            }
                        }
                    }
                    else if (VMTline.ToLower().Contains("$sequence"))
                    {
                        if (!ContentID.Equals(""))
                        {
                            Console.WriteLine(VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 1).ToLower() + ".smd");
                            if (Directory.Exists(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + "_anims\\"))
                            {
                                if (!File.Exists(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + "_anims\\" + VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 1).ToLower() + ".smd"))
                                {
                                    sequenceSet = true;
                                    ignoreSeq = true;
                                }
                            }
                            else
                            {
                                sequenceSet = true;
                                ignoreSeq = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine(VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 1).ToLower() + ".smd");
                            if (Directory.Exists(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + "_anims\\"))
                            {
                                if (!File.Exists(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + "_anims\\" + VMTline.Substring(VMTline.IndexOf("\"") + 1, VMTline.LastIndexOf("\"") - VMTline.IndexOf("\"") - 1).ToLower() + ".smd"))
                                {
                                    sequenceSet = true;
                                    ignoreSeq = true;
                                }
                            }
                            else
                            {
                                sequenceSet = true;
                                ignoreSeq = true;
                            }
                        }
                    }

                    if (sequenceSet) {
                        if (VMTline.Contains("{"))
                        {
                            sequenceLevel++;
                            ignoreSeq = false;
                        }

                        if (VMTline.Contains("}"))
                        {
                            sequenceLevel--;
                            if(sequenceLevel <= 0 && ignoreSeq == false)
                            {
                                sequenceSet = false;
                            }
                        }

                        VMTline = "";
                    }

                    newFile.Add(VMTline);
                }
                vmt.Close();

                System.IO.StreamWriter file;
                if (!ContentID.Equals(""))
                {
                    using (file =
                    new System.IO.StreamWriter(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".qc", false))
                    {
                        foreach (string line in newFile)
                        {
                            file.WriteLine(line);
                        }
                    }
                }
                else
                {
                    using (file =
                    new System.IO.StreamWriter(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".qc", false))
                    {
                        foreach (string line in newFile)
                        {
                            file.WriteLine(line);
                        }
                    }
                }

                string arg;
                if (!ContentID.Equals(""))
                {
                    arg = "-game \"" + SteamLibPath + "\\GarrysMod\\garrysmod\" -basedir \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\" -nop4 -verbose";
                }
                else
                {
                    arg = "-game \"" + SteamLibPath + "\\GarrysMod\\garrysmod\" -basedir \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf("/")).Replace("models/", "") + "\" -nop4 -verbose";
                }
                if (MSLOD)
                {
                    if (MSLODF.Count() == 0)
                    {
                        arg = arg + " -striplods";
                    }
                    else
                    {
                        foreach (string LODFilter in MSLODF)
                        {
                            string sToC = entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "");
                            if (sToC.Contains(LODFilter))
                            {
                                arg = arg + " -striplods";
                                break;
                            }
                        }
                    }
                }
                if (!ContentID.Equals(""))
                {
                    arg = arg + " \"" + OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".qc\"";
                }
                else
                {
                    arg = arg + " \"" + OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".qc\"";
                }
                RecompileModel(SteamLibPath, arg);
                if (!ContentID.Equals(""))
                {
                    Console.WriteLine(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Replace("models/", ""));
                    Console.WriteLine(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Replace("models/", ""));
                    Console.WriteLine("--------------------------------------------------");
                }
                else
                {
                    Console.WriteLine(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Replace("models/", ""));
                    Console.WriteLine(OutputPath + "\\models\\" + entry.Key.Replace("models/", ""));
                    Console.WriteLine("--------------------------------------------------");
                }
                if (!ContentID.Equals(""))
                {
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Replace("models/", ""), OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Replace("models/", ""), true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd", true);
                    }
                    catch (FileNotFoundException)
                    { }
                }
                else {
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Replace("models/", ""), OutputPath + "\\models\\" + entry.Key.Replace("models/", ""), true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy", true);
                    }
                    catch (FileNotFoundException)
                    { }
                    try
                    {
                        File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd", true);
                    }
                    catch (FileNotFoundException)
                    { }
                }

                currentVal++;
                App.Current.Dispatcher.Invoke(
                    (Action<int, int, string>)((VMFLineCounte, VMFLine, fileName) =>
                    {
                        window.ImportStatusText.Content = "Porting Models (Step 2/3) (" + VMFLineCounte.ToString() + "/" + VMFLine.ToString() + ") - " + fileName;
                        double CB = (double)VMFLineCounte / VMFLine;
                        window.ImportCurrentPB.Value = CB * (double)100;
                        window.ImportOverallPB.Value = (CB * (double)12.5) + 77.5;
                    }),
                    new object[] { currentVal, modelCount, entry.Key }
                );
            }
        }


        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        static void DirClear(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        if (f.EndsWith(".qc") || f.EndsWith(".smd"))
                        {
                            File.Delete(f);
                        }
                    }
                    if (d.EndsWith("_anims"))
                    {
                        Directory.Delete(d);
                    }
                    else
                    {
                        DirClear(d);
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
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


        public void VMFThirdRunHandler(
           string OutputPath,
           string ContentID,
           string VMFPath,
           Dictionary<string, string> soundImportList,
           Dictionary<string, string> materialImportList,
           Dictionary<string, string> modelImportList,
           MainWindow window
        )
        {
            int VMFLines = 0;
            using (StreamReader r = new StreamReader(VMFPath))
            {
                while (r.ReadLine() != null) { VMFLines++; }
            }

            System.IO.StreamReader vmf = new System.IO.StreamReader(VMFPath);
            string VMFline;
            List<string> newFile = new List<string>();
            int VMFLineCounter = 0;
            while ((VMFline = vmf.ReadLine()) != null)
            {
                string txtLine = VMFline;

                if (VMFline.ToLower().Contains(".mp3") || VMFline.ToLower().Contains(".wav"))
                {
                    //sound handler (3rd run)
                    Tuple<string, string, string> resInfo = GetResourceInfo(VMFline);
                    if (soundImportList.ContainsKey(resInfo.Item1))
                    {
                        if (!ContentID.Equals(""))
                        {
                            txtLine = txtLine.Replace(resInfo.Item1, ContentID + "/" + resInfo.Item1);
                        }
                    }
                }
                else if (VMFline.Contains("material") && VMFline.Contains("detailmaterial") == false || VMFline.Contains("texture") && VMFline.Contains("TextureScale") == false)
                {
                    //Material Handler (3rd run)
                    Tuple<string, string, string> resInfo = GetResourceInfo(VMFline);
                    if (materialImportList.ContainsKey(resInfo.Item1.ToLower()))
                    {
                        if (!ContentID.Equals(""))
                        {
                            txtLine = txtLine.Replace(resInfo.Item1, ContentID.ToUpper() + "/" + resInfo.Item1);
                        }
                    }
                }
                else if (VMFline.Contains("model") && VMFline.ToLower().Contains(".mdl"))
                {
                    //Model Handler (3rd run)
                    Tuple<string, string, string> resInfo = GetResourceInfo(VMFline);
                    if (modelImportList.ContainsKey(resInfo.Item1.ToLower()))
                    {
                        if (resInfo.Item2.Length < 7)
                        {
                            if (!ContentID.Equals(""))
                            {
                                txtLine = txtLine.Replace(resInfo.Item1, "models/" + ContentID + "/" + resInfo.Item3);
                            }
                            else
                            {
                                txtLine = txtLine.Replace(resInfo.Item1, "models/" + resInfo.Item3);
                            }
                        }
                        else
                        {
                            if (!ContentID.Equals(""))
                            {
                                txtLine = txtLine.Replace(resInfo.Item1, "models/" + ContentID + "/" + resInfo.Item2.Substring(7) + "/" + resInfo.Item3);
                            }
                            else
                            {
                                txtLine = txtLine.Replace(resInfo.Item1, "models/" + resInfo.Item2.Substring(7) + "/" + resInfo.Item3);
                            }
                        }
                    }
                }
                else if (VMFline.Contains("model") && VMFline.ToLower().Contains(".vmt"))
                {
                    //Model Handler (3rd run)
                    Tuple<string, string, string> resInfo = GetResourceInfo(VMFline);
                    if (modelImportList.ContainsKey(resInfo.Item1.ToLower().Replace(".vmt", "")))
                    {
                        if (resInfo.Item2.Length < 7)
                        {
                            if (!ContentID.Equals(""))
                            {
                                txtLine = txtLine.Replace(resInfo.Item1.Replace(".vmt", ""), "models/" + ContentID + "/" + resInfo.Item3.Replace(".vmt", ""));
                            }
                            else
                            {
                                txtLine = txtLine.Replace(resInfo.Item1.Replace(".vmt", ""), "models/" + resInfo.Item3.Replace(".vmt", ""));
                            }
                        }
                        else
                        {
                            if (!ContentID.Equals(""))
                            {
                                txtLine = txtLine.Replace(resInfo.Item1.Replace(".vmt", ""), "models/" + ContentID + "/" + resInfo.Item2.Substring(7) + "/" + resInfo.Item3.Replace(".vmt", ""));
                            }
                            else
                            {
                                txtLine = txtLine.Replace(resInfo.Item1.Replace(".vmt", ""), "models/" + resInfo.Item2.Substring(7) + "/" + resInfo.Item3.Replace(".vmt", ""));
                            }
                        }
                    }
                }

                newFile.Add(txtLine);

                VMFLineCounter++;
                App.Current.Dispatcher.Invoke(
                    (Action<int, int>)((VMFLineCounte, VMFLine) =>
                    {
                        window.ImportStatusText.Content = "Creating New VMF (Step 3/3) (" + VMFLineCounte.ToString() + "/" + (VMFLine * 2).ToString() + ")";
                        double CB = (double)VMFLineCounte / VMFLine;
                        window.ImportCurrentPB.Value = CB * (double)50;
                        window.ImportOverallPB.Value = (CB * (double)2.5) + 95;
                    }),
                    new object[] { VMFLineCounter, VMFLines }
                );
            }
            vmf.Close();

            VMFLineCounter = 0;
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(VMFPath.Substring(0, VMFPath.LastIndexOf(".vmf")) + "_cp.vmf", false))
            {
                foreach (string line in newFile)
                {
                    file.WriteLine(line);

                    VMFLineCounter++;
                    App.Current.Dispatcher.Invoke(
                    (Action<int, int>)((VMFLineCounte, VMFLine) =>
                    {
                        window.ImportStatusText.Content = "Creating New VMF (Step 3/3) (" + (VMFLineCounte + VMFLine).ToString() + "/" + (VMFLine * 2).ToString() + ")";
                        double CB = (double)(VMFLineCounte + VMFLine) / (VMFLine * 2);
                        window.ImportCurrentPB.Value = CB * (double)100;
                        window.ImportOverallPB.Value = (CB * (double)5) + 95;
                    }),
                    new object[] { VMFLineCounter, VMFLines }
                );
                }
            }
        }



        /*
        * 
        * Content Pack Generator - Thread/Thread Creator
        * 
        */

        public Thread SourceImportThreaded(
            string VMFPath,
            string SteamLibPath,
            string OutputPath,
            string IncludePath,
            string ContentID,
            bool ModelStripLOD,
            List<string> MSLODF,
            bool MapSkipStepThree,
            MainWindow window
        )
        {
            var t = new Thread(() => SourceImportThreadedReal(VMFPath, SteamLibPath, OutputPath, IncludePath, ContentID, ModelStripLOD, MSLODF, MapSkipStepThree, window));
            t.Start();
            return t;
        }

        private void SourceImportThreadedReal(
            string VMFPath,
            string SteamLibPath,
            string OutputPath,
            string IncludePath,
            string ContentID,
            bool ModelStripLOD,
            List<string> MSLODF,
            bool MapSkipStepThree,
            MainWindow window
        )
        {
            //lets keep track of time
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Build out list of supported source engine VPKs
            App.Current.Dispatcher.Invoke(() =>
            {
                window.ImportStatusText.Content = "Building Library";
            });

            BuildLibraryList(SteamLibPath, window);

            Dictionary<string, string> soundImportList = new Dictionary<string, string>();
            Dictionary<string, string> materialImportList = new Dictionary<string, string>();
            Dictionary<string, string> textureImportList = new Dictionary<string, string>();
            Dictionary<string, string> modelImportList = new Dictionary<string, string>();

            //make sure the library has stuff in it
            if (foundGames.Count() > 0)
            {
                //build model dictionaries
                string gmodReqDir = SteamLibPath + "\\GarrysMod\\bin";
                if (Directory.Exists(gmodReqDir))
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        window.ImportOverallPB.Value = 10;
                        window.ImportCurrentPB.Value = 0;
                    });

                    //Begin Initial VMF Scan (get all info first into list of need-to-import, then import+edit during 2nd run)
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        window.ImportStatusText.Content = "Scanning VMF (Step 1/3)";
                        window.ImportCurrentPB.Value = 0;
                    });

                    int VMFLines = 0;
                    using (StreamReader r = new StreamReader(VMFPath))
                    {
                        while (r.ReadLine() != null) { VMFLines++; }
                    }

                    System.IO.StreamReader vmf = new System.IO.StreamReader(VMFPath);
                    string VMFline;
                    int VMFLineCounter = 0;
                    while ((VMFline = vmf.ReadLine()) != null)
                    {
                        Tuple<Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>> VFRResponse = VMFFirstRunHandler(VMFline, soundImportList, materialImportList, textureImportList, modelImportList, SteamLibPath, OutputPath);

                        soundImportList = VFRResponse.Item1;
                        materialImportList = VFRResponse.Item2;
                        textureImportList = VFRResponse.Item3;
                        modelImportList = VFRResponse.Item4;

                        VMFLineCounter++;
                        App.Current.Dispatcher.Invoke(
                            (Action<int, int>)((VMFLineCounte, VMFLine) =>
                            {
                                window.ImportStatusText.Content = "Scanning VMF (Step 1/3) (" + VMFLineCounte.ToString() + "/" + VMFLine.ToString() + ")";
                                double CB = (double)VMFLineCounte / VMFLine;
                                window.ImportCurrentPB.Value = CB * (double)100;
                                window.ImportOverallPB.Value = (CB * (double)30) + 10;
                            }),
                            new object[] { VMFLineCounter, VMFLines }
                        ); 
                    }
                    vmf.Close(); 

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        window.ImportOverallPB.Value = 40;
                        window.ImportCurrentPB.Value = 0;
                        window.ImportStatusText.Content = "Porting Content (Step 2/3)";
                    });

                    //transfer+edit files as needed
                    VMFSecondRunHandler(soundImportList, materialImportList, textureImportList, modelImportList, SteamLibPath, OutputPath, ContentID, ModelStripLOD, MSLODF, window);

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        window.ImportOverallPB.Value = 90;
                        window.ImportCurrentPB.Value = 0;
                        window.ImportStatusText.Content = "Porting Content (Step 2/3) - Cleanup";
                    });

                    //cleanup stuff
                    //delete temp folder
                    if (Directory.Exists(OutputPath + "\\temp"))
                    {
                        try
                        {
                            Directory.Delete(OutputPath + "\\temp", true);
                        }
                        catch (IOException) { }
                        try
                        {
                            Directory.Delete(OutputPath + "\\temp");
                        }
                        catch (IOException) { }
                    }

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        window.ImportOverallPB.Value = 92.5;
                        window.ImportCurrentPB.Value = 50;
                        window.ImportStatusText.Content = "Porting Content (Step 2/3) - Cleanup and Includes";
                    });

                    //scan models folder and delete .qc and .smd files
                    DirClear(OutputPath + "\\models\\");

                    if (IncludePath != "")
                    {
                        Copy(IncludePath, OutputPath);
                        Copy(IncludePath, SteamLibPath + "\\GarrysMod\\garrysmod\\");
                    }

                    //re-copy all generated models
                    foreach (KeyValuePair<string, string> entry in modelImportList)
                    {
                        if (!ContentID.Equals(""))
                        {
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Replace("models/", ""));
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd");
                            }
                            catch (FileNotFoundException)
                            { }



                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Replace("models/", ""), OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Replace("models/", ""), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd", OutputPath + "\\models\\" + ContentID + "\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd", true);
                            }
                            catch (FileNotFoundException)
                            { }
                        }
                        else
                        {
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Replace("models/", ""));
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy");
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Delete(OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd");
                            }
                            catch (FileNotFoundException)
                            { }



                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Replace("models/", ""), OutputPath + "\\models\\" + entry.Key.Replace("models/", ""), true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vtx", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".jpg", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".360.vtx", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx80.vtx", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".dx90.vtx", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.vvd", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".sw.phy", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".phy", true);
                            }
                            catch (FileNotFoundException)
                            { }
                            try
                            {
                                File.Copy(SteamLibPath + "\\GarrysMod\\garrysmod\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd", OutputPath + "\\models\\" + entry.Key.Substring(0, entry.Key.LastIndexOf(".mdl")).Replace("models/", "") + ".vvd", true);
                            }
                            catch (FileNotFoundException)
                            { }
                        }
                    }

                    if (!MapSkipStepThree)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            window.ImportOverallPB.Value = 95;
                            window.ImportCurrentPB.Value = 0;
                            window.ImportStatusText.Content = "Creating New VMF (Step 3/3)";
                        });

                        //copy VMF and replace old values with new ones
                        VMFThirdRunHandler(OutputPath, ContentID, VMFPath, soundImportList, materialImportList, modelImportList, window);
                    }

                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;

                    App.Current.Dispatcher.Invoke(
                        (Action<TimeSpan>)((tss) =>
                        {
                            window.ImportOverallPB.Value = 0;
                            window.ImportCurrentPB.Value = 0;
                            window.ImportStatusText.Content = "Success! Completed in " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", tss.Hours, tss.Minutes, tss.Seconds, tss.Milliseconds / 10);
                            window.ImportStart.IsEnabled = true;
                        }),
                        new object[] { ts }
                    );

                    if (!MapSkipStepThree)
                    {
                        window.showMSGBox("Content importing was successful!\n\nYour Content Pack is located in:\n" + OutputPath + "\n\nYour modified VMF is at:\n" + VMFPath.Substring(0, VMFPath.LastIndexOf(".vmf")) + "_cp.vmf", "Content Import Success");
                    }
                    else
                    {
                        window.showMSGBox("Content importing was successful!\n\nYour Content Pack is located in:\n" + OutputPath, "Content Import Success");
                    }
                }
            }
        }
    }
}
