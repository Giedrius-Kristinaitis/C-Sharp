/*
 * Author: Giedrius Kristinaitis
 * 
 * For the sake of simplicity all classes are kept in the same source file
 */

using System;
using System.IO;

namespace Modules {

    /// <summary>
    /// main program's class
    /// </summary>
    class Program {

        // maximum number of modules in the data file
        const int MaxNumberOfModules = 50;

        /// <summary>
        /// entry point of the program
        /// </summary>
        /// <param name="args">arguments for the program</param>
        static void Main(string[] args) {
            ModuleList allModules = ReadData("U2.txt");

            WriteInitialDataToFile("PradDuomenys.txt", allModules);

            FindDependentModules(allModules);
            ModuleListContainer lists = FindAllPosibleModuleLists(allModules);

            SaveResults("Rezultatai.txt", lists);
        }

        /// <summary>
        /// writes initial data to a given file
        /// </summary>
        /// <param name="fileName">file name to write to</param>
        /// <param name="allModules">ModuleList containing all modules which will be written to the file</param>
        static void WriteInitialDataToFile(string fileName, ModuleList allModules) {
            using (StreamWriter writer = new StreamWriter(fileName)) {
                writer.WriteLine("{0, -14} {1, -34} {2}", "Modulio kodas", "| Modulio pavadinimas", "| Reikia išklausyti modulius");
                writer.WriteLine("------------------------------------------------------------------------------");

                for (int i = 0; i < allModules.Count; i++) {
                    Module module = allModules.GetModule(i);

                    writer.Write("{0, -14} {1, -34}", module.Code, "| " + module.Name + "   | ");

                    string[] neededModules = module.GetNeededModules();

                    for (int j = 0; j < neededModules.Length; j++) {
                        writer.Write("{0} ", neededModules[j]);
                    }

                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// writes results (all possible module lists) to a file
        /// </summary>
        /// <param name="fileName">file name to write to</param>
        /// <param name="lists">all possible module lists</param>
        static void SaveResults(string fileName, ModuleListContainer lists) {
            using (StreamWriter writer = new StreamWriter(fileName)) {
                if (lists == null || lists.Count == 0) {
                    writer.WriteLine("Nėra įmanomų modulių priklausomybės sąrašų");
                } else {
                    writer.WriteLine("Visi įmanomi modulių priklausomybės sąrašai:");

                    for (int i = 0; i < lists.Count; i++) {
                        ModuleList list = lists.GetModuleList(i);

                        for (int j = 0; j < list.Count; j++) {
                            writer.WriteLine("{0} {1}", list.GetModule(j).Code, list.GetModule(j).Name);
                        }

                        writer.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// loops through all modules and for each module, finds all modules that depend on it
        /// </summary>
        /// <param name="modules">module list to loop through</param>
        static void FindDependentModules(ModuleList modules) {
			for (int i = 0; i < modules.Count; i++) {
                foreach (string module in modules.GetModule(i).GetNeededModules()) {
                    modules.GetModuleByCode(module).DependentModules[
                        modules.GetModuleByCode(module).DependentModulesCount++] = modules.GetModule(i).Code;
                }
            }
        }

        /// <summary>
        /// finds all possible module dependency lists
        /// </summary>
        /// <param name="modules">list of all modules</param>
        /// <returns>ModuleListContainer with all possible lists in it</returns>
        static ModuleListContainer FindAllPosibleModuleLists(ModuleList modules) {
            if (ZeroListsPossible(modules)) {
                return null;
            }
            
            ModuleListContainer moduleLists = new ModuleListContainer(10000);

            for (int i = 0; i < modules.Count; i++) {
                CreateModuleLists(moduleLists, modules, modules.GetModule(i));
            }
            
            return moduleLists;
        }

        /// <summary>
        /// creates all possible lists where specific module is in positions from
        /// it's needed modules count to the number of modules that depend on it
        /// </summary>
        /// <param name="lists">module list container containing all formed dependency lists</param>
        /// <param name="modules">all modules that can be chosen</param>
        /// <param name="module">module which will be placed in specific positions</param>
        static void CreateModuleLists(ModuleListContainer lists, ModuleList modules, Module module) {
            int neededModulesCount = module.GetNeededModules().Length;
            int dependentModulesCount = module.DependentModulesCount;
            
            for (int j = neededModulesCount; j < modules.Count - dependentModulesCount; j++) {
                ModuleList list = new ModuleList(modules.Count);
                ModuleList unnecessaryModules = FindUnnecessaryModules(modules, module);
                AddNeededModulesToTheList(modules, list, module.GetNeededModules());
                list.AddAll(unnecessaryModules);
                list.MoveModuleTo(module, j);
                
                AddDependentModulesToTheList(modules, list, module.DependentModules, module.DependentModulesCount);
                
                if (!lists.Contains(list) && ValidList(list)) {
                    lists.AddModuleList(list);
                }
            }
        }

        /// <summary>
        /// checks if a module list is valid (modules which need other modules can't be placed before them)
        /// </summary>
        /// <param name="list">module list to check</param>
        /// <returns>true if the list is valid, false otherwise</returns>
        static bool ValidList(ModuleList list) {
            for (int i = 0; i < list.Count; i++) {
                Module module = list.GetModule(i);

                for (int j = i; j < list.Count; j++) {
                    if (ArrayContainsElement(module.GetNeededModules(),
                        module.GetNeededModules().Length, list.GetModule(j).Code)) {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// finds modules that do not depend on or are not needed by specific module
        /// </summary>
        /// <param name="allModules">all modules</param>
        /// <param name="module">module for which unnecessary modules will be found</param>
        /// <returns>module list containing modules that are not important for the given module</returns>
        static ModuleList FindUnnecessaryModules(ModuleList allModules, Module module) {
            ModuleList unnecessary = new ModuleList(allModules.Count);

            for (int i = 0; i < allModules.Count; i++) {
                string moduleCode = allModules.GetModule(i).Code;

                if (!ArrayContainsElement(module.GetNeededModules(), module.GetNeededModules().Length, moduleCode) 
                    && !ArrayContainsElement(module.DependentModules, module.DependentModulesCount, moduleCode)) {
                    unnecessary.AddModule(allModules.GetModule(i));
                }
            }

            return unnecessary;
        }

        /// <summary>
        /// checks if a string array contains specific element
        /// </summary>
        /// <param name="array">array to check for element</param>
        /// <param name="length">length of the array</param>
        /// <param name="element">element to check for</param>
        /// <returns>true if the element exists in the array</returns>
        static bool ArrayContainsElement(string[] array, int length, string element) {
            for (int i = 0; i < length; i++) {
                if (array[i].Equals(element)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// adds needed modules of some module to a module list
        /// </summary>
        /// <param name="allModules">all modules</param>
        /// <param name="list">module list to add to</param>
        /// <param name="neededModules">modules that will be added to the list</param>
        static void AddNeededModulesToTheList(ModuleList allModules, ModuleList list, string[] neededModules) {
            for (int i = 0; i < neededModules.Length; i++) {
                list.AddModule(allModules.GetModuleByCode(neededModules[i]));
            }
        }

        /// <summary>
        /// adds modules that depend on some module to a module list
        /// </summary>
        /// <param name="allModules">all modules</param>
        /// <param name="list">module list to add to</param>
        /// <param name="dependentModules">modules which will be added to the list</param>
        /// <param name="moduleCount">number of modules that will be added</param>
        static void AddDependentModulesToTheList(ModuleList allModules, ModuleList list, 
            string[] dependentModules, int moduleCount) {

            for (int i = 0; i < moduleCount; i++) {
                list.AddModule(allModules.GetModuleByCode(dependentModules[i]));
            }
        }

        /// <summary>
        /// checks if no possible module dependency lists can be created
        /// </summary>
        /// <param name="modules">all modules</param>
        /// <returns>true if no lists are possible</returns>
        static bool ZeroListsPossible(ModuleList modules) {
            for (int i = 0; i < modules.Count; i++) {
                if (modules.GetModule(i).GetType() == typeof(Module)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// reads module data from a given file
        /// </summary>
        /// <param name="fileName">file name to read from</param>
        /// <returns>ModuleList containing modules which were found in the file</returns>
        static ModuleList ReadData(string fileName) {
            ModuleList modules = new ModuleList(MaxNumberOfModules);

            using (StreamReader reader = new StreamReader(fileName)) {
                int moduleCount = int.Parse(reader.ReadLine());

                for (int i = 0; i < moduleCount; i++) {
                    string line = reader.ReadLine();
                    string code = line.Substring(0, 4);
                    string name = line.Substring(5, 30);
                    int neededModulesCount = int.Parse(line.Substring(36, 1));

                    if (neededModulesCount > 0) {
                        string[] neededModules = line.Substring(38).Split(new char[] {' '}, 
							StringSplitOptions.RemoveEmptyEntries);
                        modules.AddModule(new DependentModule(code, name, neededModulesCount, neededModules));
                    } else {
                        modules.AddModule(new Module(code, name));
                    }
                }
            }

            return modules;
        }
    }


    /// <summary>
    /// class that stored Module objects
    /// </summary>
    class ModuleList {
        
        public int Count { get; private set; } // number of modules that currently are in the container
        private Module[] Modules; // all modules that this container stores

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="capacity">maximun number of modules this container can store</param>
        public ModuleList(int capacity) {
            Modules = new Module[capacity];
            Count = 0;
        }

        /// <summary>
        /// adds a module to the end of the container
        /// because all modules in the list must be unique, module
        /// can only be added if it doesn't already exist in this container 
        /// </summary>
        /// <param name="module">module to add</param>
        /// <returns>true if the module was successfully added to the list</returns>
        public bool AddModule(Module module) {
            if (!ContainsModule(module)) {
                Modules[Count++] = module;
                return true;
            }

            return false;
        }

        /// <summary>
        /// adds all modules from given list to this container
        /// </summary>
        /// <param name="list">list containing modules to add</param>
        public void AddAll(ModuleList list) {
            for (int i = 0; i < list.Count; i++) {
                AddModule(list.GetModule(i));
            }
        }

        /// <summary>
        /// checks if this container contains a specific module
        /// </summary>
        /// <param name="module">module to check for</param>
        /// <returns>true if the module is found in the container</returns>
        public bool ContainsModule(Module module) {
            for (int i = 0; i < Count; i++) {
                if (Modules[i] == module) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// gets module at the specified index
        /// </summary>
        /// <param name="index">position to take module from</param>
        /// <returns>module at the given index</returns>
        public Module GetModule(int index) {
            return Modules[index];
        }

        /// <summary>
        /// returns module that matches the given module code
        /// </summary>
        /// <param name="code">code of the module to search for</param>
        /// <returns>Module with the specified code, null if no module is found</returns>
        public Module GetModuleByCode(string code) {
            for (int i = 0; i < Count; i++) {
                if (Modules[i].Code.Equals(code)) {
                    return Modules[i];
                }
            }

            return null;
        }

        /// <summary>
        /// moves specific module to the specified index
        /// and places the module at the given index in the position of the 
        /// specified module
        /// </summary>
        /// <param name="module">module to move</param>
        /// <param name="index">index to which the module will be moved</param>
        public void MoveModuleTo(Module module, int index) {
            for (int i = 0; i < Count; i++) {
                if (module == Modules[i]) {
                    Module temp = Modules[index];
                    Modules[index] = Modules[i];
                    Modules[i] = temp;
                }
            }
        }
    }

    
    /// <summary>
    /// class that stores ModuleList objects
    /// </summary>
    class ModuleListContainer {

        public int Count { get; set; } // number of module lists that currently are in the container
        private ModuleList[] ModuleLists; // module lists this container stores

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="capacity">maximum number of lists this container will be able to store</param>
        public ModuleListContainer(int capacity) {
            ModuleLists = new ModuleList[capacity];
        }

        /// <summary>
        /// adds a module list to the end of the container
        /// </summary>
        /// <param name="list"></param>
        public void AddModuleList(ModuleList list) {
            ModuleLists[Count++] = list;
        }

        /// <summary>
        /// gets a module list
        /// </summary>
        /// <param name="index">index of wanted module list</param>
        /// <returns>module list at specified index</returns>
        public ModuleList GetModuleList(int index) {
            return ModuleLists[index];
        }

        /// <summary>
        /// checks if a this module list container contains a specific list
        /// </summary>
        /// <param name="list">list to check for</param>
        /// <returns>true if the given list exists in this container</returns>
        public bool Contains(ModuleList list) {
            for (int i = 0; i < Count; i++) {
                for (int j = 0; j < list.Count; j++) {
                    bool equal = ListsEqual(ModuleLists[i], list);

                    if (equal) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// checks if two module list are equal (each element value and position matches)
        /// </summary>
        /// <param name="one">first list of the comparison</param>
        /// <param name="two">second list of the comparison</param>
        /// <returns>true if module lists are equal</returns>
        public bool ListsEqual(ModuleList one, ModuleList two) {
            if (one.Count != two.Count) {
                return false;
            }

            for (int i = 0; i < one.Count; i++) {
                if (one.GetModule(i) != two.GetModule(i)) {
                    return false;
                }
            }
            
            return true;
        }
    }


    /// <summary>
    /// class containing information about a module
    /// </summary>
    class Module {

        public string Code { get; set; } // code of the module
        public string Name { get; set; } // name of the module
        public int DependentModulesCount { get; set; } // number of modules that depend on this module
        public string[] DependentModules { get; set; } // all modules' codes that depend on this module

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="code">code of the module</param>
        /// <param name="name">full name of the module</param>
        public Module(string code, string name) {
            Code = code;
            Name = name;
            DependentModules = new string[50];
        }
        
        /// <summary>
        /// checks if a module is equal to this module
        /// modules are considered equal if their codes match
        /// </summary>
        /// <param name="module">module to check for equality</param>
        /// <returns>true if modules are equal</returns>
        public bool Equals(Module module) {
            return Code.Equals(module.Code);
        }

        /// <summary>
        /// overrides default Equals() of the object class
        /// </summary>
        /// <param name="obj">object to check for equality</param>
        /// <returns>true if obj is equal to this object</returns>
        public override bool Equals(object obj) {
            if (object.ReferenceEquals(obj, null)) {
                return false;
            }

            if (obj.GetType() != this.GetType()) {
                return false;
            }

            return this.Equals(obj as Module);
        }

        /// <summary>
        /// overrides default GetHashCode() of the object class
        /// hash code is based on the code of the module
        /// </summary>
        /// <returns>hash code of the module</returns>
        public override int GetHashCode() {
            return Code.GetHashCode();
        }

        /// <summary>
        /// checks if two modules are equal
        /// </summary>
        /// <param name="lhs">left-hand-side module of the comparison</param>
        /// <param name="rhs">right-hand-side module of the comparison</param>
        /// <returns>true if modules are equal</returns>
        public static bool operator == (Module lhs, Module rhs) {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// checks if two modules are not equal
        /// </summary>
        /// <param name="lhs">left-hand-side module of the comparison</param>
        /// <param name="rhs">right-hand-side module of the comparison</param>
        /// <returns>true if modules are not equal</returns>
        public static bool operator != (Module lhs, Module rhs) {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// this is default implementation of the method which can be overriden by child class
        /// returns all modules that need to be chosen before this one can be chosen
        /// because this module doesnt depend on any other modules, an empty array is returned
        /// </summary>
        /// <returns>empty string array</returns>
        public virtual string[] GetNeededModules() {
            return new string[0];
        }
    }


    /// <summary>
    /// class containing information about module that depends on other modules
    /// </summary>
    class DependentModule : Module {

        public int NeededModulesCount { get; set; } // number of needed modules
        private string[] NeededModules; // needed modules array with codes

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="code">code of the module</param>
        /// <param name="name">full name of the module</param>
        /// <param name="neededModulesCount">number of modules that need to be chosen before this one can be chosen</param>
        /// <param name="neededModules">string array containing all needed modules' codes</param>
        public DependentModule(string code, string name, int neededModulesCount, string[] neededModules)
            : base(code, name) {

            NeededModulesCount = neededModulesCount;
            NeededModules = neededModules;
        }

        /// <summary>
        /// returns all modules that need to be chosen before this one can be chosen
        /// </summary>
        /// <returns>string array containing needed modules' codes</returns>
        public override string[] GetNeededModules() {
            return NeededModules;
        }
    }
}
