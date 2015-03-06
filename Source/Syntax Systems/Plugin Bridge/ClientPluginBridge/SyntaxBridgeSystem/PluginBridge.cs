using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DarkMultiPlayer;
using UnityEngine;
using SyntaxBridgeSystem.PluginHandling;
using System.IO;
using System.Reflection;

namespace SyntaxBridgeSystem
{
    namespace PluginHandling
    {
        [KSPAddon(KSPAddon.Startup.EveryScene,false)]
        public class PluginBridgeThread : MonoBehaviour
        {
            private static readonly List<IBridge> pluginBridges = new List<IBridge>();
            static bool isRunning = false;
            static bool fullyInitialized = false;
            static bool baseInitialized = false;
            static bool isCrashed = false;
            
            public void LoadSyntaxGUI()
            {

            }
            public PluginBridgeThread()
            {

                if (HighLogic.LoadedScene == GameScenes.SPACECENTER | HighLogic.LoadedScene == GameScenes.FLIGHT | HighLogic.LoadedScene == GameScenes.TRACKSTATION)
                {


                    if (PluginBridge.Initialize())
                    {
                        Debug.Log("PluginBridge Initialized");
                        isRunning = true;
                    }
                    if (isRunning)
                    {
                        if (PluginBridge.InititalizePlugins())
                        {
                            baseInitialized = true;
                            Debug.Log("PluginBridge: Base classes initialized.");
                        }
                    }
                    if (!baseInitialized)
                    {
                        Debug.Log("PluginBridge initialization has failed.");
                        return;
                    }
                    while (!fullyInitialized)
                    {
                        // start the plugins
                        try
                        {
                            LoadPlugins();
                            Debug.Log("PluginBridge: Plugins loaded!");
                            Debug.Log("PluginBridge: Initializing plugins..");
                            foreach (var plugin in pluginBridges)
                            {
                                try
                                {

                                    plugin.InitializePlugin();
                                }
                                catch (Exception e)
                                {
                                    Type type = plugin.GetType();
                                    Debug.Log("Error thrown in OnServerStop event for " + type.FullName + " (" + type.Assembly.FullName + "), Exception: " + e);
                                }
                            }
                            Debug.Log("PluginBridge: plugins successfully initialized!");
                            fullyInitialized = true;
                        }
                        catch (Exception ex)
                        {
                            fullyInitialized = false;
                            isCrashed = true;
                            Debug.Log("PluginBridge: Initialization of plugins failed. Errorcode: " + ex.ToString());
                        }
                        if (isCrashed)
                        {
                            Debug.Log("PluginBridge: Crashed during initialization of plugins");
                            break;
                        }
                    }
                    if (fullyInitialized)
                    {
                        LoadSyntaxGUI();
                        Debug.Log("Syntax GUI loaded.");
                    }
                }
            }
            public static void LoadPlugins()
            {
                string pluginDirectory = Path.Combine(Path.Combine(Environment.CurrentDirectory,"GameData"),Path.Combine("DarkMultiPlayer","Plugins"));
                if (!Directory.Exists(pluginDirectory))
                {
                    Directory.CreateDirectory(pluginDirectory);
                }
                Debug.Log("Loading plugins!");
                //Load all the assemblies just in case they depend on each other during instantation
                List<Assembly> loadedAssemblies = new List<Assembly>();
                string[] pluginFiles = Directory.GetFiles(pluginDirectory, "*", SearchOption.AllDirectories);
                foreach (string pluginFile in pluginFiles)
                {
                    if (Path.GetExtension(pluginFile).ToLower() == ".dll" && pluginFile != Path.Combine(pluginDirectory,"ICSharpCode.SharpZipLib.dll") && pluginFile != Path.Combine(pluginDirectory,"DarkMultiPlayer.dll") && pluginFile != Path.Combine(pluginDirectory,"DarkMultiPlayer-Common.dll") && pluginFile != Path.Combine(pluginDirectory,"MessageWriter2.dll") && pluginFile != Path.Combine(pluginDirectory,"AntiCheatSystem.dll") && pluginFile != Path.Combine(pluginDirectory,"SyntaxBridgeSystem.dll"))
                    {
                        Debug.Log("Iteration of pluginfiles: Current filename: " + pluginFile);
                        try
                        {
                            //UnsafeLoadFrom will not throw an exception if the dll is marked as unsafe, such as downloaded from internet in Windows
                            //See http://stackoverflow.com/a/15238782
                            Assembly loadedAssembly = Assembly.LoadFile(Path.Combine(pluginDirectory,pluginFile));
                            loadedAssemblies.Add(loadedAssembly);
                            Debug.Log("Loaded " + pluginFile);
                        }
                        catch (NotSupportedException)
                        {
                            //This should only occur if using Assembly.LoadFrom() above instead of Assembly.UnsafeLoadFrom()
                            Debug.Log("Can't load dll, perhaps it is blocked: " + pluginFile);
                        }
                        catch
                        {
                            Debug.Log("Error loading " + pluginFile);
                        }
                    }
                }

                //Iterate through the assemblies looking for classes that have the IDMPPlugin interface

                Type bridgeInterfaceType = typeof(IBridge);

                foreach (Assembly loadedAssembly in loadedAssemblies)
                {
                    Type[] loadedTypes = loadedAssembly.GetExportedTypes();
                    foreach (Type loadedType in loadedTypes)
                    {
                        Type[] typeInterfaces = loadedType.GetInterfaces();
                        bool containsDMPInterface = false;
                        foreach (Type typeInterface in typeInterfaces)
                        {
                            if (typeInterface == bridgeInterfaceType)
                            {
                                containsDMPInterface = true;
                            }
                        }
                        if (containsDMPInterface)
                        {
                            Debug.Log("Loading plugin: " + loadedType.FullName);

                            try
                            {
                                IBridge pluginInstance = ActivatePluginType(loadedType);

                                if (pluginInstance != null)
                                {
                                    Debug.Log("Loaded plugin: " + loadedType.FullName);

                                    pluginBridges.Add(pluginInstance);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.Log("Error loading plugin " + loadedType.FullName + "(" + loadedType.Assembly.FullName + ") Exception: " + ex.ToString());
                            }
                        }
                    }
                }
                Debug.Log("Done!");
            }

            private static IPluginBridge ActivatePluginType(Type loadedType)
            {
                try
                {
                    IPluginBridge pluginInstance = Activator.CreateInstance(loadedType) as IPluginBridge;
                    return pluginInstance;
                }
                catch (Exception e)
                {
                    Debug.Log("Cannot activate plugin " + loadedType.Name + ", Exception: " + e.ToString());
                    return null;
                }
            }
        }
        //public class InterfaceBridger : PluginBridge.IPluginBridge
        //{
        //    public void Initializer();
        //}

        public static class PluginBridge
        {
            //public delegate void PluginBridgeHandler(string pluginName, byte[] messagedata);
            public delegate void PluginBridgeHandler(byte[] messagedata);
            private static Dictionary<string, List<byte[]>> pluginData = new Dictionary<string, List<byte[]>>();
            internal static bool Initialize()
            {
                bool flag = false;
                try
                {
                    DarkMultiPlayer.DMPModInterface.fetch.RegisterRawModHandler("PLUGINBRIDGE", new DMPMessageCallback(pluginMessageReader));
                    Debug.Log("PluginBridge: Initialized.");
                    AntiCheatSystemHandler.RegisterAntiCheatPlugin();
                    Debug.Log("AntiCheat: Initialized.");
                    //RegisterSyntaxPlugins();
                    flag = true;
                }
                catch(Exception ex)
                {
                    flag = false;
                    Debug.Log("PluginBridge: Initialization failed. Errorcode: " + ex.ToString());
                }
                return flag;
            }
            
            internal static bool InititalizePlugins()
            {
                bool flag = false;
                try
                {
                    

                    flag = true;
                }
                catch(Exception ex)
                {
                    Debug.Log("PluginBridge: SyntaxCodes Initialization failed. Errorcode: " + ex.ToString());
                    flag = false;
                }
                return flag;
            }
            public static List<byte[]> PluginMessageHandler(string pluginName)
            {
                if(pluginData.ContainsKey(pluginName))
                {
                    List<byte[]> returnData = pluginData[pluginName];
                    pluginData[pluginName].Clear();
                    return returnData;
                }
                return null;
            }

            /// <summary>
            /// Internal use only. Reads all incomming messages from hooked in plugins on the pluginbridge.
            /// </summary>
            /// <param name="messagedata">The messagebytes to read</param>
            private static void pluginMessageReader(byte[] messagedata)
            {
                string pluginname;
                
                using (MessageStream2.MessageReader mr = new MessageStream2.MessageReader(messagedata))
                {
                    pluginname = mr.Read<string>();
                }
                
                if(!pluginData.ContainsKey(pluginname))
                {
                    pluginData.Add(pluginname, new List<byte[]>());
                    pluginData[pluginname].Add(messagedata);
                }
                else
                {
                    if(pluginData[pluginname] == null)
                    {
                        pluginData[pluginname] = new List<byte[]>();
                        pluginData[pluginname].Add(messagedata);
                    }
                    else
                    {
                        pluginData[pluginname].Add(messagedata);
                    }
                }
            }

            /// <summary>
            /// Hook in your plugin here. Specify your code void to receive server response.
            /// </summary>
            /// <param name="pluginName">Your pluginname without spaces</param>
            /// <param name="yourPluginVoidResponseHandler">Your public plugin void handler</param>
            public static void New(string pluginName, PluginBridgeHandler yourPluginVoidResponseHandler )
            {
                pluginData.Add(pluginName,null);
                DarkMultiPlayer.DMPModInterface.fetch.RegisterRawModHandler(pluginName, new DMPMessageCallback(yourPluginVoidResponseHandler));
            }

            /// <summary>
            /// Send a plugin message via your own plugin.
            /// </summary>
            /// <param name="pluginname">Your pluginname</param>
            /// <param name="messagedata">byte data to send</param>
            /// <param name="relayToClients">Wether to relay the message to clients</param>
            /// <param name="priorityHigh">Wether the message has high priority or not</param>
            public static void SendPluginMessage(string pluginname, byte[] messagedata, bool relayToClients, bool priorityHigh)
            {
                DMPModInterface.fetch.SendDMPModMessage(pluginname, messagedata, relayToClients, priorityHigh);
            }

            /// <summary>
            /// Send a message to the server via the pluginbridge. Remember to retrieve your data via PluginMessageHandler or PluginMessageReader.
            /// </summary>
            /// <param name="pluginname">Your pluginname</param>
            /// <param name="messagedata">byte data to send</param>
            /// <param name="relayToClients">Wether to relay the message to clients</param>
            /// <param name="priorityHigh">Wether the message has high priority or not</param>
            public static void SendSyntaxPluginMessage(string pluginname, byte[] messagedata, bool relayToClients, bool priorityHigh)
            {
                byte[] finalizedData;
                using(MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
                {
                    mw.Write<string>(pluginname);
                    mw.Write<byte[]>(messagedata);
                    finalizedData = mw.GetMessageBytes();
                }
                DMPModInterface.fetch.SendDMPModMessage("PLUGINBRIDGE", messagedata, relayToClients, priorityHigh);
            }

            //private static void RegisterSyntaxPlugins()
            //{
            //    // Anti Cheat System bootup
            //    // Permission System bootup
                
            //    // Trading System bootup

            //}
        }
    }
}
