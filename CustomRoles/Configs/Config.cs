using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;

namespace CustomRoles.Configs
{
    using Exiled.Loader.Features.Configs;
    using Exiled.Loader.Features.Configs.CustomConverters;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using YamlDotNet.Serialization.NodeDeserializers;

    public class Config : IConfig
    {
        public Roles RoleConfigs;

        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;
        
        [Description("Whether or not debug messages shoudl be shown.")]
        public bool Debug { get; set; } = true;
        
        [Description("The folder path where role configs will be stored.")]
        public string RolesFolder { get; set; } = Path.Combine(Paths.Configs, "CustomRoles");
        
        [Description("The file name to load role configs from.")]
        public string RolesFile { get; set; } = "global.yml";

        [Description("A list of zombie class names that are enabled on this server.")]
        public List<string> EnabledZombies { get; set; } = new List<string>();
        
        private ISerializer Serializer = new SerializerBuilder().WithTypeConverter(new VectorsConverter())
            .WithTypeInspector(i => new CommentGatheringTypeInspector(i))
            .WithEmissionPhaseObjectGraphVisitor(a => new CommentsObjectGraphVisitor(a.InnerVisitor))
            .WithNamingConvention(UnderscoredNamingConvention.Instance).IgnoreFields().Build();
        
        public static IDeserializer Deserializer = new DeserializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();

        public void LoadConfigs()
        {
            if (!Directory.Exists(RolesFolder))
                Directory.CreateDirectory(RolesFolder);

            string filePath = Path.Combine(RolesFolder, RolesFile);
            Log.Info($"{filePath}");
            if (!File.Exists(filePath))
            {
                RoleConfigs = new Roles();
                File.WriteAllText(filePath, Serializer.Serialize(RoleConfigs));
            }
            else
            {
                RoleConfigs = Deserializer.Deserialize<Roles>(File.ReadAllText(filePath));
                File.WriteAllText(filePath, Serializer.Serialize(RoleConfigs));
            }
        }
    }
}