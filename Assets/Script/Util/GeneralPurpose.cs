using System.Collections;
using System.Collections.Generic;
using System.IO; // ←追加
using System.Runtime.Serialization.Json; // ←追加
using UnityEngine;
using System.Text;

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using System.Runtime.Serialization;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace GeneralPurpose {


public class GeneralPurpose : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public static class JsonUtils
{
    /// <summary>
    /// JSONからオブジェクトへ変換します
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json">オブジェクトへ変換するJSON</param>
    /// <returns>オブジェクト</returns>
    public static T ToObject<T>(string json)
    {
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(ms);
        }
    }
}


[DataContract]
public class ErrorResponse
{
    [DataMember]
    public string ErrorMessage;
}
/*
public class Dbg{
    public static void properties<T>(T obj, string objname="OBJECT"){
        Type type = obj.GetType();


        string output = objname;
        output += "\n---Properties---\n";

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (PropertyInfo p in properties) {
            output += p.Name;

            PropertyInfo prop = type.GetProperty(p.Name);
            //output += prop.get;


            output += "\n";
        }

        Debug.Log(output);
    }

}
*/

//test class
public class Planet
{
   private String planetName;
   private Double distanceFromEarth;
   
   public Planet(String name, Double distance)
   {
      planetName = name;
      distanceFromEarth = distance;
   } 

   public String Name
   { get { return planetName; } }
   
   public Double Distance 
   { get { return distanceFromEarth; }
     set { distanceFromEarth = value; } }
}


public class Dbg
{
    //こいつにオブジェクト投げればとりあえず色々表示してくれるよ
    public static void s(object obj, string objectname="OBJECTINFO"){
        Dbg.p(obj, objectname);
        Dbg.m(obj, objectname);
    }

   
    public static void p(object obj, string objectname="OBJECT_PROPERTY_INFO"){
        string output = objectname;
        output += "\n---Properties---\n";

        Type t = obj.GetType();
        output += "--class name: <";
        output += t.Name;
        output += ">--\n";

        PropertyInfo[] props = t.GetProperties();
        
        output += "--number of properties: <";
        output += props.Length;
        output += ">--\n";

        foreach (PropertyInfo prop in props){
            if (prop.GetIndexParameters().Length == 0){
                output += (prop.Name);
                output += "((";
                output += (prop.PropertyType.Name);
                output += ")): ";
                output += (prop.GetValue(obj));
                output += "\n\n";
            }else{
                output += (prop.Name);
                output += "((";
                output += prop.PropertyType.Name;
                output += ")) is GetIndexParameters != 0";
            }
        }

        Debug.Log(output);
    }

    // 参考　https://docs.microsoft.com/ja-jp/dotnet/api/system.reflection.methodinfo?view=netcore-3.1
    public static void m(object obj, string objectname="OBJECT_METHOD_INFO"){
        Type type = obj.GetType();
        MethodInfo[] methods = type.GetMethods();

        string output = objectname;
        output +="\n------methodinfo-------\n";

        output += "--class name: <";
        output += type.Name;
        output += ">--\n";

        output += "--number of methods: <";
        output += methods.Length;
        output += ">--\n\n";

        foreach (MethodInfo m in methods) {
            output += m.Name;
            output += "\n((is_private)): ";
            output += m.IsPrivate;
            output += "\n((is_static)): ";
            output += m.IsStatic;

            output += "\n<paras>: ";

            ParameterInfo[] paras = m.GetParameters();
            
            foreach (ParameterInfo para in paras){
                output += para;
                output += ", ";
            }

            output += "\n→ ";
            output += m.ReturnType;

            output += "\n\n";
        }


            

/*
            Type aaa = m.GetType();
            MethodInfo[] m2i = aaa.GetMethods();
            foreach (MethodInfo bb in m2i){
                Debug.Log(bb.Name);
            }

            Dbg.p(m);
        }
*/
        Debug.Log(output);
    }
}

}


