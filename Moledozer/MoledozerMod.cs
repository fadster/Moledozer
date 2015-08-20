using ColossalFramework.UI;
using ColossalFramework;
using ICities;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Moledozer
{
    public class MoledozerMod : LoadingExtensionBase, IUserMod
    {
        public string Name { get { return "Moledozer"; } }
        public string Description { get { return "Like a bulldozer, but only underground!"; } }

        private RedirectCallsState redirectState;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame) {
                redirectState = RedirectionHelper.RedirectCalls(
                    typeof(BulldozeTool).GetMethod("GetService", BindingFlags.Instance | BindingFlags.Public),
                    typeof(MoledozeTool).GetMethod("GetService", BindingFlags.Instance | BindingFlags.Public));
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            RedirectionHelper.RevertRedirect(redirectState);
        }
    }
        
    public class MoledozeTool : DefaultTool
    {
        public override ToolBase.RaycastService GetService()
        {
            ItemClass.Layer trafficLayer = (this.GetType() != typeof(BulldozeTool)) ?
                ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.Markers :
                ItemClass.Layer.MetroTunnels;

            ItemClass.Availability mode = Singleton<ToolManager>.instance.m_properties.m_mode;

            if ((mode & ItemClass.Availability.MapAndAsset) != ItemClass.Availability.None) {
                InfoManager.InfoMode currentMode = Singleton<InfoManager>.instance.CurrentMode;
                if (currentMode == InfoManager.InfoMode.Transport) {
                    return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None,
                        ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths | ItemClass.Layer.Markers);
                }
                if (currentMode != InfoManager.InfoMode.Traffic) {
                    return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.Markers);
                }
                return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, trafficLayer | ItemClass.Layer.Markers);
            } else {
                InfoManager.InfoMode currentMode = Singleton<InfoManager>.instance.CurrentMode;
                if (currentMode == InfoManager.InfoMode.Water) {
                    return new ToolBase.RaycastService(ItemClass.Service.Water, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.WaterPipes);
                }
                if (currentMode == InfoManager.InfoMode.Transport) {
                    return new ToolBase.RaycastService(ItemClass.Service.PublicTransport, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels);
                }
                if (currentMode != InfoManager.InfoMode.Traffic) {
                    return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default);
                }
                return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, trafficLayer);
            }
        }
    }
}