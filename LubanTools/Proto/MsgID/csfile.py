from const import csfile_path


def genCSfile(protos):
    fileContent = ""
    fileContent += ('\n'
                    'using Google.Protobuf;\n'
                    'using Msg;\n'
                    'using System;\n'
                    'using System.Collections.Generic;\n\n'
                    'namespace Proto\n'
                    '{\n'
                    '   public class ProtoDic\n'
                    '   {\n')

    fileContent += '      private static Dictionary<string,Type> _name2Type = new Dictionary<string,Type>()\n      {\n'

    for index in range(len(protos)):
          fileContent = fileContent + '''           {"Msg.''' + protos[index] + '"'',typeof(Msg.''' + protos[index] + ''')},\n'''

    fileContent += ('       };\n\n'
                    '       private static readonly Dictionary<RuntimeTypeHandle, MessageParser> Parsers = new Dictionary<RuntimeTypeHandle, MessageParser>()\n'
                    '       {\n')

    for index in range(len(protos)):
        fileContent = fileContent + '''            {typeof(''' + protos[index] + ''').TypeHandle, Msg.''' + protos[index] + '''.Parser },\n'''

    fileContent += ('       };\n\n        public static MessageParser GetMessageParser(RuntimeTypeHandle typeHandle)\n'
                   '        {\n'
                   '            Parsers.TryGetValue(typeHandle, out var messageParser);\n'
                   '            return messageParser;\n'
                   '        }\n'
                   '\n'
                   '        public static Type GetProtoTypeByName(string name)\n'
                   '        {\n'
                   '            return _name2Type.GetValueOrDefault(name);\n'
                   '        }\n'
                   '\n'
                   '\n'
                   '        public static IMessage ParseBytesData(byte[] data, Type type)\n'
                   '        {\n'
                   '            MessageParser messageParse = GetMessageParser(type.TypeHandle);\n'
                   '            return messageParse.ParseFrom(data);\n'
                   '        }\n'
                   '\n'
                   '\n'
                   '        public static bool ContainName(string name)\n'
                   '        {\n'
                   '            if(_name2Type.ContainsKey(name))\n'
                   '            {\n'
                   '                return true;\n'
                   '            }\n'
                   '            return false;\n'
                   '        }\n'
                   '\n'
                   '    }\n'
                   '}')

    fo = open(csfile_path, "wb")
    fo.write(fileContent.encode('utf-8'))
    fo.close()
