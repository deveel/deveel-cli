using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Deveel.Configuration {
	class ReflectedOptions {
		private static bool FilterMember(MemberInfo member, object criteria) {
			//if (!Attribute.IsDefined(member, typeof(OptionAttribute)))
			//	return false;
			
			if (member is PropertyInfo) {
				var prop = (PropertyInfo)member;
				return prop.CanRead && prop.CanWrite;
			}
			
			return true;
		}
		
		private static Option CreateOptionFromMember(MemberInfo member, IDictionary<string, OptionGroup> groups) {
			var attribute = (OptionAttribute) Attribute.GetCustomAttribute(member, typeof(OptionAttribute));
			
			string optionName = null;
			string optionLongName = null;
			if (attribute != null) {
				optionLongName = attribute.LongName;
				optionName = attribute.Name;
			}

			if (String.IsNullOrEmpty(optionName))
				optionName = member.Name[0].ToString(CultureInfo.InvariantCulture);
			if (String.IsNullOrEmpty(optionLongName))
				optionLongName = member.Name;

			var desc = attribute != null ? attribute.Description : null;
			var option = new Option(optionName, desc) {
				LongName = optionLongName,
				ArgumentCount = attribute != null ? attribute.ArgumentCount : 0,
				IsRequired = attribute != null && attribute.IsRequired,
				ArgumentName = attribute != null ? attribute.ArgumentName : "arg",
				ValueSeparator = attribute != null ? attribute.ValueSeparator : ' ',
				HasOptionalArgument = attribute != null && attribute.HasOptionalArgument
			};

			var groupAttr = (OptionGroupAttribute)Attribute.GetCustomAttribute(member, typeof(OptionGroupAttribute));
			if (groupAttr != null) {
				OptionGroup group;
				if (!groups.TryGetValue(groupAttr.Name, out group)) {
					group = new OptionGroup();
					groups[groupAttr.Name] = group;
				}

				group.AddOption(option);
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
			
			//if (!Attribute.IsDefined(type, typeof(OptionsAttribute)))
			//	throw new ArgumentException("The type '" + type + "' is not marked as options");
			
			MemberInfo[] members = type.FindMembers(MemberTypes.Field | MemberTypes.Property, 
			                                        BindingFlags.Public | BindingFlags.Instance, 
			                                        FilterMember, null);
			
			var groups = new Dictionary<string, OptionGroup>();
			var requiredGroups = new List<string>();
			
			Attribute[] groupsAttrs = Attribute.GetCustomAttributes(type, typeof(OptionGroupAttribute));
			foreach (OptionGroupAttribute groupAttr in groupsAttrs) {
				OptionGroup group;
				if (!groups.TryGetValue(groupAttr.Name, out @group)) {
					@group = new OptionGroup {IsRequired = groupAttr.IsRequired};
					groups[groupAttr.Name] = @group;
					if (groupAttr.IsRequired)
						requiredGroups.Add(groupAttr.Name);
				}
			}
			
			var options = new Options();
			
			foreach (MemberInfo member in members) {
				Option option = CreateOptionFromMember(member, groups);
				if (option != null)
					options.AddOption(option);
			}
			
			foreach(var entry in groups) {
				var group = entry.Value;				
				options.AddOptionGroup(group);
			}
			
			return options;
		}
		
		public static void SetToObject(Options options, ICommandLine cmdLine, object obj) {
			if (obj == null)
				return;

			var type = obj.GetType();
			MemberInfo[] members = type.FindMembers(MemberTypes.Field | MemberTypes.Property,
										BindingFlags.Public | BindingFlags.Instance,
										FilterMember, null);

			foreach (var member in members) {
				SetOptionsToMember(member, options, cmdLine);
			}
		}

		private static void SetOptionsToMember(MemberInfo member, Options options, ICommandLine cmdLine) {
			var optionName = member.Name;

			var attrs = member.GetCustomAttributes(typeof (OptionAttribute), false);
			if (attrs.Length > 0) {
				var attr = (OptionAttribute) attrs[0];
				optionName = attr.Name;
				if (String.IsNullOrEmpty(optionName))
					optionName = attr.LongName;
				if (String.IsNullOrEmpty(optionName))
					optionName = member.Name;
			}

			if (cmdLine.HasOption(optionName)) {
				var value = cmdLine.GetOptionValue(optionName);
			}
		}
	}
}