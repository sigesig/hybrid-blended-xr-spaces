// NOTE: In a shipping apllication it is inefficient to do this since the
// Update() dispatch is an unnecessary expense. It will often be the case
// that a scene will be loaded with an IBL setup, and this will not change
// until the next scene is loaded.
#if UNITY_EDITOR
#define ALLOW_DYNAMIC_IBL_SWTICH
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Avatar2
{

    public class IblSetupForEnvironment : MonoBehaviour
    {
        [Tooltip("This value should affect all tone mapped and non tone mapped lighting exposures.")]
        public float CurrentExposure = 1.0f;
        private float _previousExposure;
        [Tooltip("Exposure shader parameter name.")]
        public string ExposureShaderParameterName = "u_Exposure";

        [Tooltip("A prefiltered single mip texture for diffuse influences.")]
        public Texture DiffuseEnvironmentCubeMap;
        private Texture _previousDiffuseEnvironmentCubeMap;
        [Tooltip("Diffuse environment sampler shader parameter name.")]
        public string DiffuseEnvironmentShaderParameterName = "u_DiffuseEnvSampler";

        [Tooltip("Ambient specular reflections on a multiple mip texture, chosen via the roughness of materials.")]
        public Texture SpecularEnvironmentCubeMap;
        private Texture _previousSpecularEnvironmentCubeMap;
        [Tooltip("Specular environment sampler shader parameter name.")]
        [SerializeField]
        private string SpecularEnvironmentShaderParameterName = "u_SpecularEnvSampler";
        [Tooltip("Represents the number of mips in the specular texture map. This is automatically set when setting the texture.")]
        public string SpecularMapMipCountShaderPameterName = "u_MipCount";

        [Tooltip("A 2 channel look up textrure to represent the BRDF function.")]
        public Texture BrdfLutMap;
        private Texture _previousBrdfLutMap;
        [Tooltip("BRDF Lut sammpler shader parameter name.")]
        public string BrdfLutShaderParameterName = "u_brdfLUT";

        [Tooltip("Optional world object cubemap material, the specular environment cube above will be set into it.")]
        public Material CubeMapMaterial;
        [Tooltip("Name of the texture parameter in the world object cubemap shader.")]
        public string CubeMapShaderParameterName = "_Tex";


        private void SetExposureScopeParm() {
            Shader.SetGlobalFloat(ExposureShaderParameterName, CurrentExposure);
            _previousExposure = CurrentExposure;
        }

        private void SetAllIblGlobalScopeParams() {
            Shader.SetGlobalTexture(DiffuseEnvironmentShaderParameterName, DiffuseEnvironmentCubeMap);
            Shader.SetGlobalTexture(SpecularEnvironmentShaderParameterName, SpecularEnvironmentCubeMap);
            Shader.SetGlobalTexture(BrdfLutShaderParameterName, BrdfLutMap);
#if UNITY_2018
            Shader.SetGlobalInt(SpecularMapMipCountShaderPameterName, 10);
#else
            Shader.SetGlobalInt(SpecularMapMipCountShaderPameterName, SpecularEnvironmentCubeMap.mipmapCount);
#endif
            if (CubeMapMaterial != null)
            {
                CubeMapMaterial.SetTexture(CubeMapShaderParameterName, SpecularEnvironmentCubeMap);
            }


            _previousDiffuseEnvironmentCubeMap = DiffuseEnvironmentCubeMap;
            _previousSpecularEnvironmentCubeMap = SpecularEnvironmentCubeMap;
            _previousBrdfLutMap = BrdfLutMap;
        }

        private void OnEnable()
        {
            SetExposureScopeParm();
            SetAllIblGlobalScopeParams();
        }

        private void OnDisable()
        {

        }

#if ALLOW_DYNAMIC_IBL_SWTICH
        private void Update()
        {
            if(CurrentExposure != _previousExposure) {
                SetExposureScopeParm();
            }
            if ( _previousDiffuseEnvironmentCubeMap != DiffuseEnvironmentCubeMap ||
                _previousSpecularEnvironmentCubeMap != SpecularEnvironmentCubeMap ||
                _previousBrdfLutMap != BrdfLutMap ) 
            {
                SetAllIblGlobalScopeParams();
            }
        }
#endif
    }
}
