//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;


namespace cfg
{
   
public sealed class Tables
{
    public Common.TbGlobalConfig TbGlobalConfig {get; private set; }
    public Error.TbErrorInfo TbErrorInfo {get; private set; }
    public Error.TbCodeInfo TbCodeInfo {get; private set; }
    public Configs.TbSounds_Config TbSounds_Config {get; private set; }
    public Configs.TbLanguage_Config TbLanguage_Config {get; private set; }
    public Configs.TbUIData_GameMode TbUIData_GameMode {get; private set; }
    public Configs.TbUIData_Race TbUIData_Race {get; private set; }
    public Configs.TbUIData_Character TbUIData_Character {get; private set; }
    public Configs.TbPlayerData_Character TbPlayerData_Character {get; private set; }
    public Configs.TbEntityData TbEntityData {get; private set; }
    public Configs.TbLevelData TbLevelData {get; private set; }

    public Tables() { }
    
    public async UniTask LoadAsync(System.Func<string, UniTask<ByteBuf>> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
		List<UniTask> list = new List<UniTask>();
		list.Add(UniTask.Create(async () =>
		{
			TbGlobalConfig = new Common.TbGlobalConfig(await loader("common_tbglobalconfig")); 
			tables.Add("Common.TbGlobalConfig", TbGlobalConfig);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbErrorInfo = new Error.TbErrorInfo(await loader("error_tberrorinfo")); 
			tables.Add("Error.TbErrorInfo", TbErrorInfo);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbCodeInfo = new Error.TbCodeInfo(await loader("error_tbcodeinfo")); 
			tables.Add("Error.TbCodeInfo", TbCodeInfo);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbSounds_Config = new Configs.TbSounds_Config(await loader("configs_tbsounds_config")); 
			tables.Add("Configs.TbSounds_Config", TbSounds_Config);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbLanguage_Config = new Configs.TbLanguage_Config(await loader("configs_tblanguage_config")); 
			tables.Add("Configs.TbLanguage_Config", TbLanguage_Config);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbUIData_GameMode = new Configs.TbUIData_GameMode(await loader("configs_tbuidata_gamemode")); 
			tables.Add("Configs.TbUIData_GameMode", TbUIData_GameMode);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbUIData_Race = new Configs.TbUIData_Race(await loader("configs_tbuidata_race")); 
			tables.Add("Configs.TbUIData_Race", TbUIData_Race);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbUIData_Character = new Configs.TbUIData_Character(await loader("configs_tbuidata_character")); 
			tables.Add("Configs.TbUIData_Character", TbUIData_Character);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbPlayerData_Character = new Configs.TbPlayerData_Character(await loader("configs_tbplayerdata_character")); 
			tables.Add("Configs.TbPlayerData_Character", TbPlayerData_Character);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbEntityData = new Configs.TbEntityData(await loader("configs_tbentitydata")); 
			tables.Add("Configs.TbEntityData", TbEntityData);
		}));
		list.Add(UniTask.Create(async () =>
		{
			TbLevelData = new Configs.TbLevelData(await loader("configs_tbleveldata")); 
			tables.Add("Configs.TbLevelData", TbLevelData);
		}));

		await UniTask.WhenAll(list);

        TbGlobalConfig.Resolve(tables); 
        TbErrorInfo.Resolve(tables); 
        TbCodeInfo.Resolve(tables); 
        TbSounds_Config.Resolve(tables); 
        TbLanguage_Config.Resolve(tables); 
        TbUIData_GameMode.Resolve(tables); 
        TbUIData_Race.Resolve(tables); 
        TbUIData_Character.Resolve(tables); 
        TbPlayerData_Character.Resolve(tables); 
        TbEntityData.Resolve(tables); 
        TbLevelData.Resolve(tables); 
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbGlobalConfig.TranslateText(translator); 
        TbErrorInfo.TranslateText(translator); 
        TbCodeInfo.TranslateText(translator); 
        TbSounds_Config.TranslateText(translator); 
        TbLanguage_Config.TranslateText(translator); 
        TbUIData_GameMode.TranslateText(translator); 
        TbUIData_Race.TranslateText(translator); 
        TbUIData_Character.TranslateText(translator); 
        TbPlayerData_Character.TranslateText(translator); 
        TbEntityData.TranslateText(translator); 
        TbLevelData.TranslateText(translator); 
    }
}

}