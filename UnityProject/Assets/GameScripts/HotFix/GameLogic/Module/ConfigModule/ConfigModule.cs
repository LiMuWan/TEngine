using System;
using Bright.Serialization;
using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{
    public class ConfigModule : Singleton<ConfigModule>
    {
        private Tables tables;
        public Tables Tables => tables;
        protected override void OnInit()
        {
            base.OnInit();
            tables = new Tables();
        }

        public override void Active()
        {
            base.Active();
            // 定义一个加载器函数，用于加载数据
            async UniTask<ByteBuf> LoadTable(string tableName)
            {
                // 使用变量 result 直接返回 ByteBuf
                return new ByteBuf((GameModule.Resource.LoadAsset<TextAsset>(tableName)).bytes);
            }


            // 加载配置表
            tables.LoadAsync(LoadTable).Forget();

            // 定义一个翻译函数，用于翻译文本
            string Translate(string key, string defaultValue)
            {
                // 这里可以实现翻译逻辑，这里示例直接返回默认值
                return defaultValue; // 直接返回原文
            }

            // 进行文本翻译
            // tables.TranslateText(Translate);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            tables = null;
        }
    }
}
