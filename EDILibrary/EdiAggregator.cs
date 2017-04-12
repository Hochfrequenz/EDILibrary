using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EDILibrary
{
    public class EdiAggregator
    {
        public string AggregateFromList(List<string> ediFiles)
        {
            //zuerst mal Sortenreinheit ignorieren und von der ersten Datei übernehmen
            List<IEdiObject> ediObjects = new List<IEdiObject>();
            var loader = new EDILibrary.GenericEDILoader();
            var writer = new GenericEDIWriter();
            string treeTest = null;
            string templateTest = null;
            string createtemplateTest = null;
            foreach(string edi in ediFiles)
            {
                EDIFileInfo fileinfo = EDIHelper.GetEDIFileInfo(edi);
                if (treeTest == null)
                {
                    treeTest = "";//TODO: Template laden EDIExtensions.LoadResourceTemplate(fileinfo, "tree");
                    templateTest = "";//TODO: Template laden EDIExtensions.LoadResourceTemplate(fileinfo, "template");
                    createtemplateTest = "";//TODO: Template laden EDIExtensions.LoadResourceTemplate(fileinfo, "create.template");
                }
                XElement template = loader.LoadTemplate(templateTest);
                TreeElement tree = loader.LoadTree(treeTest);
                TreeElement edi_tree = loader.LoadEDI(edi, tree);
                TreeHelper.RefreshDirtyFlags(tree);
                ediObjects.Add(loader.LoadTemplateWithLoadedTree(template, edi_tree));
            }
            IEdiObject first = ediObjects.First();
            foreach(var ediObject in ediObjects.Skip(1))
            {
                first.Child(Repositories.EDIEnums.Nachricht).AddChild(ediObject.Child(Repositories.EDIEnums.Nachricht).Child(Repositories.EDIEnums.Vorgang));
            }

            string retEdi = writer.CompileTemplate(createtemplateTest, first);
            return retEdi;
        }
    }
}
