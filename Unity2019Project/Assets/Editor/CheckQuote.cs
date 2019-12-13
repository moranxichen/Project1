using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;

public class CheckQuote
{
    public static List<string> allFiles;

    [MenuItem("Assets/CheckTools/检查引用状态")]
    
    static void Quote()
    {
        string[] selectobj = Selection.assetGUIDs;
        //string selectname = Selection.activeObject.name;
        //Debug.Log(selectname);

        allFiles = new List<string>(Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories));

        int QuoteQuantity = 0;
        for (int i = 0; i < allFiles.Count; i++)
        {

            string filealltext = allFiles[i]; 
            if (Regex.IsMatch(File.ReadAllText(filealltext), selectobj[0]))
            {
                string[] filesArray = allFiles[i].Split('/');
                string filename = filesArray[filesArray.Length - 1];
                if (Regex.IsMatch(filename,".meta"))
                {
                    continue;
                }
                else
                {
                    Debug.Log(filename + "引用了所选文件");
                    QuoteQuantity += 1;
                }
            }
        }
        if (QuoteQuantity == 0 )
        {
            Debug.Log("没有找到相关引用!");
        }
    }

}
public static class CheckMat
{
    public static List<string> allmats;
    public static List<string> matNamesList = new List<string>();
    public static List<string> shaderNameList = new List<string>();

    [MenuItem("Assets/CheckTools/检查所有材质引用")]

    static void matcheck()
    {
        allmats = new List<string>(Directory.GetFiles(Application.dataPath, "*.mat", SearchOption.AllDirectories));


        for (int i = 0; i < allmats.Count; i++)
        {
            string matsPath = allmats[i];
            int index = matsPath.IndexOf("Assets", System.StringComparison.Ordinal);
            string matsPaths = matsPath.Substring(index, matsPath.Length - index);

            string[] matsNameArray = matsPath.Split('/');
            string matsName = matsNameArray[matsNameArray.Length - 1];
            matNamesList.Add(matsName);
            Material matload = AssetDatabase.LoadAssetAtPath<Material>(matsPaths);

            string shadername = matload.shader.name;
            shaderNameList.Add(shadername);
        }
         Write();
    }
    static void Write()
    {
        FileStream matTxt = new FileStream(Application.dataPath + "/MateCheckPath(请勿提交).txt", FileMode.Create);

        StreamWriter matTxtStreamWriter = new StreamWriter(matTxt);

        matTxtStreamWriter.Write("--------------------------" + "\r\n");
        matTxtStreamWriter.Write("------------" + "共" + allmats.Count + "个材质球" + "\r\n");

        if (allmats.Count != 0)
        {
            for (int i = 0; i < allmats.Count; i++)
            {
                matTxtStreamWriter.Write(matNamesList[i] + "=====================>"+ shaderNameList[i] + "\r\n");
            }
        }
        else
        {
            matTxtStreamWriter.Write("没有找到相关引用" + "\r\n");
        }
        matTxtStreamWriter.Flush();
        matTxtStreamWriter.Close();
        matTxt.Close();
        Debug.Log("日志输出完成"+"输出路径为:"+ Application.dataPath + "/MateCheckPath(请勿提交).txt");
    }
}
public static class CheckEffect
{
    public static List<string> allprefab;
    public static List<string> matNamesList = new List<string>();
    public static List<string> shaderNameList = new List<string>();

    [MenuItem("Assets/CheckTools/检查ParticleSystem数量")]

    static void paryicleCheck()
    {
        allprefab = new List<string>(Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories));

        int particleNumber = 0;
        for (int i = 0; i < allprefab.Count; i++)
        {
            int EffectKeywords = allprefab[i].IndexOf("effect_", System.StringComparison.Ordinal);

            if (EffectKeywords<0)
            {
                continue;
            }
            else
            {
               
                string prefabPath = allprefab[i];
                int indexprefab = prefabPath.IndexOf("Assets", System.StringComparison.Ordinal);
                string prefabPaths = prefabPath.Substring(indexprefab, prefabPath.Length - indexprefab);

                Transform prefabload = AssetDatabase.LoadAssetAtPath<Transform>(prefabPaths);

                int systemNumber = 0; 
                foreach (var t in prefabload.GetComponentsInChildren<ParticleSystem>(true))
                {
                    systemNumber=systemNumber+1;
                }
                string[] prefabnameArray = prefabPath.Split('/');
                string prefabName = prefabnameArray[prefabnameArray.Length - 1];

                if (systemNumber>=10)
                {
                    Debug.LogWarning(prefabName + "=======>" + systemNumber + " 个particle system"+"数量过多!");
                }
                else
                {
                    Debug.Log(prefabName + "=======>" + systemNumber +" 个particle system");
                }
            }
            particleNumber = particleNumber + 1;
        }
        Debug.Log("一共" + particleNumber + "个特效");
    }
}
public static class SelectFile
{

    [MenuItem("Assets/SelectTools/选择当前文件夹下所有贴图")]
    static void selectTexture()
    {
        List<string> type= new List<string>() { ".png", ".tga", ".psd", ".jpg" };
        SelectFiles(type);
    }
    [MenuItem("Assets/SelectTools/选择当前文件夹下所有材质")]
    static void selectMat()
    {
        var type = new List<string>() { ".mat" };
        SelectFiles(type);
    }
    [MenuItem("Assets/SelectTools/选择当前文件夹下所有模型")]
    static void selectModel()
    {
        var type = new List<string>() { ".fbx",".obj" };
        SelectFiles(type);
    }

    static void SelectFiles(List<string> listTypes)
    {
        string[] selectFileGuid = Selection.assetGUIDs;
        string selectFileName = AssetDatabase.GUIDToAssetPath(selectFileGuid[0]);
        int index = selectFileName.IndexOf("/", System.StringComparison.Ordinal);
        string fileFolderPath = selectFileName.Substring(index, selectFileName.Length - index);
        string[] TextureFiles = Directory.GetFiles(Application.dataPath + fileFolderPath, "*.*", SearchOption.AllDirectories).Where(s => listTypes.Contains(Path.GetExtension(s).ToLower())).ToArray();
        if (TextureFiles.Length<1)
        {
            Debug.Log("该文件夹下没有所选内容。");
        }
        else
        {
            List<string> selectPath = new List<string>();
            for (int i = 0; i < TextureFiles.Length; i++)
            {

                int indexname = TextureFiles[i].IndexOf("Assets", System.StringComparison.Ordinal);
                string filesPath = TextureFiles[i].Substring(indexname, TextureFiles[i].Length - indexname);
                if (filesPath != null)
                {
                    selectPath.Add(filesPath);
                }
            }
            List<Object> obj = new List<Object>();
            for (int i = 0; i < selectPath.Count; i++)
            {
                obj.Add(AssetDatabase.LoadAssetAtPath<Object>(selectPath[i]));
            }
            Object[] objlist = obj.ToArray();
            Debug.Log("已选中" + objlist.Length + "个资源");
            Selection.objects = objlist;
        }
    }
}
public static class CheckTextureOriginalsize
{
    [MenuItem("Assets/CheckTools/检查所有贴图原始尺寸")]
    static void TextureOriginalSize()
    {

        List<string> type = new List<string>() { ".png", ".tga", ".psd", ".jpg" };
        string[] AllTexture = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories).Where(s => type.Contains(Path.GetExtension(s).ToLower())).ToArray();
        for (int i = 0; i < AllTexture.Length; i++)
        {
            //Debug.Log(AllTexture[i]);
            string texpath = AllTexture[i];
            int index = texpath.IndexOf("Assets", System.StringComparison.Ordinal);
            string texpathout = texpath.Substring(index, texpath.Length - index);
            TextureImporter textureimporter = AssetImporter.GetAtPath(texpathout) as TextureImporter;
            int width = 0;
            int height = 0;
            GetTextureOriginalSize(textureimporter, out width, out height);
            if (width > 512 || height > 512)
            {
                Debug.LogWarning(texpathout + "贴图原始大小异常。" + width + "：" + height);
            }
        }
    }
    public static void GetTextureOriginalSize(TextureImporter ti, out int width, out int height)
    {
        if (ti == null)
        {
            width = 0;
            height = 0;
            return;
        }
        object[] args = new object[2] { 0, 0 };
        MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        mi.Invoke(ti, args);

        width = (int)args[0];
        height = (int)args[1];
    }
}
