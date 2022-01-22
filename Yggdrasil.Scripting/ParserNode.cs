using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Yggdrasil.Behaviour;

namespace Yggdrasil.Scripting
{
    public class ParserNode
    {
        public List<ParserNode> Children = new List<ParserNode>();
        public string DeclaringTypeDef;
        public string File;
        public List<ScriptedFunctionDefinition> FunctionDefinitions = new List<ScriptedFunctionDefinition>();
        public string Guid;
        public bool IsDerivedFromTypeDef;
        public bool IsTopmost;
        public List<ScriptedFunction> ScriptedFunctions = new List<ScriptedFunction>();
        public string Tag;
        public Type Type;
        public ParserNode TypeDef;
        public XmlNode Xml;

        public Node CreateInstance(Dictionary<string, ParserNode> typeDefMap,
            List<BuildError> errors)
        {
            return IsDerivedFromTypeDef
                ? CreateTypeDefInstance(typeDefMap, errors)
                : CreateStaticTypeInstance(errors);
        }

        private Node CreateTypeDefInstance(Dictionary<string, ParserNode> typeDefMap, List<BuildError> errors)
        {
            // Find the TypeDef parser node.
            if (!typeDefMap.TryGetValue(Tag, out var parserDef))
            {
                errors.Add(ParserErrorHelper.MissingTypeDefInstance(Tag, File));
                return null;
            }

            // Create an instance from the TypeDef parser node.
            var instance = parserDef.CreateInstance(typeDefMap, errors);
            if (instance == null) { return null; }

            // Set manager and guid.
            instance.Guid = Guid;
            instance.NodeType = Tag;

            // Set function values.
            foreach (var function in ScriptedFunctions) { function.SetFunctionPropertyValue(instance); }

            if (instance.Children == null) { instance.Children = new List<Node>(); }

            return instance;
        }

        private Node CreateStaticTypeInstance(List<BuildError> errors)
        {
            // Create an instance using reflection.
            Node instance;

            try
            {
                var serializer = new XmlSerializer(Type);
                instance = serializer.Deserialize(new XmlNodeReader(Xml)) as Node;
            }
            catch (Exception e)
            {
                errors.Add(ParserErrorHelper.UnableToInstantiate(Tag, File, e.Message));
                return null;
            }

            if (instance == null)
            {
                errors.Add(ParserErrorHelper.CannotCastToNode(Tag, File));
                return null;
            }

            // Set manager and guid.
            instance.Guid = Guid;

            // Set function values.
            foreach (var function in ScriptedFunctions) { function.SetFunctionPropertyValue(instance); }

            if (instance.Children == null) { instance.Children = new List<Node>(); }

            return instance;
        }
    }
}