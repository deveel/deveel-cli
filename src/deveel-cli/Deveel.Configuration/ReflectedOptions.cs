using System;
using System.Collections;
using System.Reflection;

namespace Deveel.Configuration {
	class ReflectedOptions {
		private static bool FilterMember(MemberInfo member, object criteria) {
			if (!Attribute.IsDefined(member, typeof(OptionAttribute)))
				return false;
			
			if (member is PropertyInfo) {
				PropertyInfo prop = (PropertyInfo)member;
				return prop.CanRead && prop.CanWrite;
			}
			
			return true;
		}
		
		private static Option CreateOptionFromMember(MemberInfo member, IDictionary groups) {
			OptionAttribute attribute = (OptionAttribute) Attribute.GetCustomAttribute(member, typeof(OptionAttribute));
			
			string optionName = attribute.Name;
			string optionLongName = attribute.LongName;
			if (optionName == null || optionName.Length == 0) {
				optionLongName = member.Name;
				optionName = optionLongName[0].ToString();
			}
			
			Option option = new Option(optionName, attribute.Description);
			option.LongName = optionLongName;
			option.ArgumentCount = attribute.ArgumentCount;
			option.IsRequired = attribute.IsRequired;
			option.ArgumentName = attribute.ArgumentName;
			option.ValueSeparator = attribute.ValueSeparator;
			option.HasOptionalArgument = attribute.HasOptionalArgument;
			
			OptionGroupAttribute groupAttr = (OptionGroupAttribute)Attribute.GetCustomAttribute(member, typeof(OptionGroupAttribute));
			if (groupAttr != null && groups.Contains(groupAttr.Name)) {
				ArrayList options = (ArrayList)groups[groupAttr.Name];
				options.Add(option);
				return null;
			}
			
			return option;
		}
		
		public static Options CreateFromObject(object obj) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			
			return CreateFromType(obj.GetType());
		}
		
		public static Options CreateFromType(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");
			
			if (!Attribute.IsDefined(type, typeof(OptionsAttribute)))
				throw new ArgumentException("The type '" + type + "' is not marked as options");
			
			MemberInfo[] members = type.FindMembers(MemberTypes.Field | MemberTypes.Property, 
			                                        BindingFlags.Public | BindingFlags.Instance, 
			                                        FilterMember, null);
			
			Hashtable groups = new Hashtable();
			ArrayList requiredGroups = new ArrayList();
			
			object[] groupsAttrs = Attribute.GetCustomAttributes(type, typeof(OptionGroupAttribute));
			for(int i = 0; i < groupsAttrs.Length; i++) {
				OptionGroupAttribute groupAttr = (OptionGroupAttribute) groupsAttrs[i];
				if (!groups.Contains(groupAttr.Name)) {
					groups.Add(groupAttr.Name, new ArrayList());
					if (groupAttr.IsRequired)
						requiredGroups.Add(groupAttr.Name);
				}
			}
			
			Options options = new Options();
			
			for (int i = 0; i < members.Length; i++) {
				Option option = CreateOptionFromMember(members[i], groups);
				if (option != null)
					options.AddOption(option);
			}
			
			foreach(DictionaryEntry entry in groups) {
				string groupName = (string)entry.Key;
				ArrayList groupOptions = (ArrayList)entry.Value;
				
				OptionGroup group = new OptionGroup();
				group.IsRequired = requiredGroups.Contains(groupName);
				for(int i = 0; i < groupOptions.Count; i++) {
					group.AddOption((Option)groupOptions[i]);
				}
				
				options.AddOptionGroup(group);
			}
			
			return options;
		}
		
		public static void SetToObject(Options options, CommandLine cmdLine, object obj) {
			throw new NotImplementedException();
		}
	}
}