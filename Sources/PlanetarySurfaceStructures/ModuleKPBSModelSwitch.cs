using UnityEngine;
using System.Collections.Generic;
using System.Text;
using KSP.Localization;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSModelSwitch : PartModule, IModuleInfo
    {

        //the names of the transforms
        [KSPField]
        public string transformNames = string.Empty;

        [KSPField]
        public string transformVisibleNames = string.Empty;

        //--------------persistent states----------------
        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "#LOC_KPBS.modelswitch.model")]
        [UI_ChooseOption(scene = UI_Scene.Editor)]
        public int numModel = 0;


        //The previous model index
        private int oldModelNum = -1;

        //The list of transforms
        private List<List<Transform>> transformsData;

        List<string> visibleNames;

        public bool initialized = false;

        BaseField modelBaseField;
        UI_ChooseOption modelUIChooser;

        //the part that is enabled and disabled
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            modelBaseField = Fields["numModel"];
            modelUIChooser = (UI_ChooseOption)modelBaseField.uiControlEditor;

            string[] transformGroupNames = transformNames.Split(',');
            string[] transformGroupVisibleNames = transformVisibleNames.Split(',');

            transformsData = new List<List<Transform>>();

            //Search in the named transforms for the lights
            if (transformNames != string.Empty)
            {
                string[] transforms = transformNames.Split(',');

                //find all the transforms
                List<Transform> transformsList = new List<Transform>();

                for (int i = 0; i < transforms.Length; i++)
                {
                    List<Transform> transSetting = new List<Transform>();
                    //get all the transforms
                    transSetting.AddRange(part.FindModelTransforms(transforms[i].Trim()));

                    transformsData.Add(transSetting);
                }

                string[] visible = transformVisibleNames.Split(',');
                for (int i = 0; i < visible.Length; i++)
                {
                    visible[i] = visible[i].Trim();
                }

                if (visible.Length == transforms.Length)
                {
                    modelUIChooser.options = visible;
                }
                else
                {
                    //set the changes for the modelchooser
                    modelUIChooser.options = transforms;
                }

                //when there is only one model, we do not need to show the controls
                if (transformNames.Length < 2)
                {
                    modelBaseField.guiActive = false;
                    modelBaseField.guiActiveEditor = false;
                }
            }
            else
            {
                Debug.LogError("ModuleKPBSModelSwitch: No transforms defined!)");
            }
        }

        /**
         * The update method of the module
         */
        public void Update()
        {
            //when the active model changes
            if (oldModelNum != numModel)
            {
                for (int i = 0; i < transformsData.Count; i++)
                {
                    if (i == numModel)
                    {
                        for (int j = 0; j < transformsData[i].Count; j++)
                        {
                            transformsData[i][j].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < transformsData[i].Count; j++)
                        {
                            transformsData[i][j].gameObject.SetActive(false);
                        }
                    }
                }
                oldModelNum = numModel;
            }
        }


        /// <summary>
        /// Get the description shown for this resource 
        /// </summary>
        /// <returns>The description of the module</returns>
        public override string GetInfo()
        {
            StringBuilder info = new StringBuilder();

            string[] transformGroupNames = transformNames.Split(',');
            string[] transformGroupVisibleNames = transformVisibleNames.Split(',');
            visibleNames = new List<string>();

            //----------------------------------------------------------
            //create the list of transforms to be made switchable
            //----------------------------------------------------------
            for (int k = 0; k < transformGroupNames.Length; k++)
            {
                if (transformGroupVisibleNames.Length == transformGroupNames.Length)
                {
                    visibleNames.Add(transformGroupVisibleNames[k]);
                }
                else
                {
                    visibleNames.Add(transformGroupNames[k]);
                }
            }

            if (visibleNames.Count > 1)
            {
                info.AppendLine(Localizer.GetStringByTag("#LOC_KPBS.modelswitch.models"));
                info.AppendLine();

                for(int i = 0; i < visibleNames.Count; i++)
                {
                    info.Append("• ");
                    info.Append(visibleNames[i]);
                    info.AppendLine();
                }
            }
            return info.ToString();
        }

        public string GetModuleTitle()
        {
            return Localizer.GetStringByTag("#LOC_KPBS.modelswitch.name");// "Model Switch";
        }

        public Callback<Rect> GetDrawModulePanelCallback()
        {
            return null;
        }

        public string GetPrimaryField()
        {
            return null;
        }
    }
}
