using System.Collections.Generic;

namespace GameLogic
{
    public class HttpSettings
    {
        public string baseUrl;
        public Dictionary<string, string> headers;
        public IEncoder encoder; // 编码器, 可自定义实现编码/加密规则
    }
}
