using System.Collections;

namespace Pointel.Windows.Views.Common.Editor.HTMLConverter
{
    internal class HtmlSchema
    {
        private static ArrayList _htmlInlineElements;
        private static ArrayList _htmlBlockElements;
        private static ArrayList _htmlOtherOpenableElements;
        private static ArrayList _htmlEmptyElements;
        private static ArrayList _htmlElementsClosingOnParentElementEnd;
        private static ArrayList _htmlElementsClosingColgroup;
        private static ArrayList _htmlElementsClosingDD;
        private static ArrayList _htmlElementsClosingDT;
        private static ArrayList _htmlElementsClosingLI;
        private static ArrayList _htmlElementsClosingTbody;
        private static ArrayList _htmlElementsClosingTD;
        private static ArrayList _htmlElementsClosingTfoot;
        private static ArrayList _htmlElementsClosingThead;
        private static ArrayList _htmlElementsClosingTH;
        private static ArrayList _htmlElementsClosingTR;
        private static Hashtable _htmlCharacterEntities;
        static HtmlSchema()
        {
            HtmlSchema.InitializeInlineElements();
            HtmlSchema.InitializeBlockElements();
            HtmlSchema.InitializeOtherOpenableElements();
            HtmlSchema.InitializeEmptyElements();
            HtmlSchema.InitializeElementsClosingOnParentElementEnd();
            HtmlSchema.InitializeElementsClosingOnNewElementStart();
            HtmlSchema.InitializeHtmlCharacterEntities();
        }
        internal static bool IsEmptyElement(string xmlElementName)
        {
            return HtmlSchema._htmlEmptyElements.Contains(xmlElementName.ToLower());
        }
        internal static bool IsBlockElement(string xmlElementName)
        {
            return HtmlSchema._htmlBlockElements.Contains(xmlElementName);
        }
        internal static bool IsInlineElement(string xmlElementName)
        {
            return HtmlSchema._htmlInlineElements.Contains(xmlElementName);
        }
        internal static bool IsKnownOpenableElement(string xmlElementName)
        {
            return HtmlSchema._htmlOtherOpenableElements.Contains(xmlElementName);
        }
        internal static bool ClosesOnParentElementEnd(string xmlElementName)
        {
            return HtmlSchema._htmlElementsClosingOnParentElementEnd.Contains(xmlElementName.ToLower());
        }
        internal static bool ClosesOnNextElementStart(string currentElementName, string nextElementName)
        {
            switch (currentElementName)
            {
                case "colgroup":
                    return HtmlSchema._htmlElementsClosingColgroup.Contains(nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                case "dd":
                    return HtmlSchema._htmlElementsClosingDD.Contains(nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                case "dt":
                    return HtmlSchema._htmlElementsClosingDT.Contains(nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                case "li":
                    return HtmlSchema._htmlElementsClosingLI.Contains(nextElementName);
                case "p":
                    return HtmlSchema.IsBlockElement(nextElementName);
                case "tbody":
                    return HtmlSchema._htmlElementsClosingTbody.Contains(nextElementName);
                case "tfoot":
                    return HtmlSchema._htmlElementsClosingTfoot.Contains(nextElementName);
                case "thead":
                    return HtmlSchema._htmlElementsClosingThead.Contains(nextElementName);
                case "tr":
                    return HtmlSchema._htmlElementsClosingTR.Contains(nextElementName);
                case "td":
                    return HtmlSchema._htmlElementsClosingTD.Contains(nextElementName);
                case "th":
                    return HtmlSchema._htmlElementsClosingTH.Contains(nextElementName);
            }
            return false;
        }
        internal static bool IsEntity(string entityName)
        {
            return HtmlSchema._htmlCharacterEntities.Contains(entityName);
        }
        internal static char EntityCharacterValue(string entityName)
        {
            if (HtmlSchema._htmlCharacterEntities.Contains(entityName))
            {
                return (char)HtmlSchema._htmlCharacterEntities[entityName];
            }
            return '\0';
        }
        private static void InitializeInlineElements()
        {
            HtmlSchema._htmlInlineElements = new ArrayList();
            HtmlSchema._htmlInlineElements.Add("a");
            HtmlSchema._htmlInlineElements.Add("abbr");
            HtmlSchema._htmlInlineElements.Add("acronym");
            HtmlSchema._htmlInlineElements.Add("address");
            HtmlSchema._htmlInlineElements.Add("b");
            HtmlSchema._htmlInlineElements.Add("bdo");
            HtmlSchema._htmlInlineElements.Add("big");
            HtmlSchema._htmlInlineElements.Add("button");
            HtmlSchema._htmlInlineElements.Add("code");
            HtmlSchema._htmlInlineElements.Add("del");
            HtmlSchema._htmlInlineElements.Add("dfn");
            HtmlSchema._htmlInlineElements.Add("em");
            HtmlSchema._htmlInlineElements.Add("font");
            HtmlSchema._htmlInlineElements.Add("i");
            HtmlSchema._htmlInlineElements.Add("img");
            HtmlSchema._htmlInlineElements.Add("ins");
            HtmlSchema._htmlInlineElements.Add("kbd");
            HtmlSchema._htmlInlineElements.Add("label");
            HtmlSchema._htmlInlineElements.Add("legend");
            HtmlSchema._htmlInlineElements.Add("q");
            HtmlSchema._htmlInlineElements.Add("s");
            HtmlSchema._htmlInlineElements.Add("samp");
            HtmlSchema._htmlInlineElements.Add("small");
            HtmlSchema._htmlInlineElements.Add("span");
            HtmlSchema._htmlInlineElements.Add("strike");
            HtmlSchema._htmlInlineElements.Add("strong");
            HtmlSchema._htmlInlineElements.Add("sub");
            HtmlSchema._htmlInlineElements.Add("sup");
            HtmlSchema._htmlInlineElements.Add("u");
            HtmlSchema._htmlInlineElements.Add("var");
        }
        private static void InitializeBlockElements()
        {
            HtmlSchema._htmlBlockElements = new ArrayList();
            HtmlSchema._htmlBlockElements.Add("blockquote");
            HtmlSchema._htmlBlockElements.Add("body");
            HtmlSchema._htmlBlockElements.Add("caption");
            HtmlSchema._htmlBlockElements.Add("center");
            HtmlSchema._htmlBlockElements.Add("cite");
            HtmlSchema._htmlBlockElements.Add("dd");
            HtmlSchema._htmlBlockElements.Add("dir");
            HtmlSchema._htmlBlockElements.Add("div");
            HtmlSchema._htmlBlockElements.Add("dl");
            HtmlSchema._htmlBlockElements.Add("dt");
            HtmlSchema._htmlBlockElements.Add("form");
            HtmlSchema._htmlBlockElements.Add("h1");
            HtmlSchema._htmlBlockElements.Add("h2");
            HtmlSchema._htmlBlockElements.Add("h3");
            HtmlSchema._htmlBlockElements.Add("h4");
            HtmlSchema._htmlBlockElements.Add("h5");
            HtmlSchema._htmlBlockElements.Add("h6");
            HtmlSchema._htmlBlockElements.Add("html");
            HtmlSchema._htmlBlockElements.Add("li");
            HtmlSchema._htmlBlockElements.Add("menu");
            HtmlSchema._htmlBlockElements.Add("ol");
            HtmlSchema._htmlBlockElements.Add("p");
            HtmlSchema._htmlBlockElements.Add("pre");
            HtmlSchema._htmlBlockElements.Add("table");
            HtmlSchema._htmlBlockElements.Add("tbody");
            HtmlSchema._htmlBlockElements.Add("td");
            HtmlSchema._htmlBlockElements.Add("textarea");
            HtmlSchema._htmlBlockElements.Add("tfoot");
            HtmlSchema._htmlBlockElements.Add("th");
            HtmlSchema._htmlBlockElements.Add("thead");
            HtmlSchema._htmlBlockElements.Add("tr");
            HtmlSchema._htmlBlockElements.Add("tt");
            HtmlSchema._htmlBlockElements.Add("ul");
        }
        private static void InitializeEmptyElements()
        {
            HtmlSchema._htmlEmptyElements = new ArrayList();
            HtmlSchema._htmlEmptyElements.Add("area");
            HtmlSchema._htmlEmptyElements.Add("base");
            HtmlSchema._htmlEmptyElements.Add("basefont");
            HtmlSchema._htmlEmptyElements.Add("br");
            HtmlSchema._htmlEmptyElements.Add("col");
            HtmlSchema._htmlEmptyElements.Add("frame");
            HtmlSchema._htmlEmptyElements.Add("hr");
            HtmlSchema._htmlEmptyElements.Add("img");
            HtmlSchema._htmlEmptyElements.Add("input");
            HtmlSchema._htmlEmptyElements.Add("isindex");
            HtmlSchema._htmlEmptyElements.Add("link");
            HtmlSchema._htmlEmptyElements.Add("meta");
            HtmlSchema._htmlEmptyElements.Add("param");
        }
        private static void InitializeOtherOpenableElements()
        {
            HtmlSchema._htmlOtherOpenableElements = new ArrayList();
            HtmlSchema._htmlOtherOpenableElements.Add("applet");
            HtmlSchema._htmlOtherOpenableElements.Add("base");
            HtmlSchema._htmlOtherOpenableElements.Add("basefont");
            HtmlSchema._htmlOtherOpenableElements.Add("colgroup");
            HtmlSchema._htmlOtherOpenableElements.Add("fieldset");
            HtmlSchema._htmlOtherOpenableElements.Add("frameset");
            HtmlSchema._htmlOtherOpenableElements.Add("head");
            HtmlSchema._htmlOtherOpenableElements.Add("iframe");
            HtmlSchema._htmlOtherOpenableElements.Add("map");
            HtmlSchema._htmlOtherOpenableElements.Add("noframes");
            HtmlSchema._htmlOtherOpenableElements.Add("noscript");
            HtmlSchema._htmlOtherOpenableElements.Add("object");
            HtmlSchema._htmlOtherOpenableElements.Add("optgroup");
            HtmlSchema._htmlOtherOpenableElements.Add("option");
            HtmlSchema._htmlOtherOpenableElements.Add("script");
            HtmlSchema._htmlOtherOpenableElements.Add("select");
            HtmlSchema._htmlOtherOpenableElements.Add("style");
            HtmlSchema._htmlOtherOpenableElements.Add("title");
        }
        private static void InitializeElementsClosingOnParentElementEnd()
        {
            HtmlSchema._htmlElementsClosingOnParentElementEnd = new ArrayList();
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("body");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("colgroup");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("dd");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("dt");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("head");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("html");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("li");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("p");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("tbody");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("td");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("tfoot");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("thead");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("th");
            HtmlSchema._htmlElementsClosingOnParentElementEnd.Add("tr");
        }
        private static void InitializeElementsClosingOnNewElementStart()
        {
            HtmlSchema._htmlElementsClosingColgroup = new ArrayList();
            HtmlSchema._htmlElementsClosingColgroup.Add("colgroup");
            HtmlSchema._htmlElementsClosingColgroup.Add("tr");
            HtmlSchema._htmlElementsClosingColgroup.Add("thead");
            HtmlSchema._htmlElementsClosingColgroup.Add("tfoot");
            HtmlSchema._htmlElementsClosingColgroup.Add("tbody");
            HtmlSchema._htmlElementsClosingDD = new ArrayList();
            HtmlSchema._htmlElementsClosingDD.Add("dd");
            HtmlSchema._htmlElementsClosingDD.Add("dt");
            HtmlSchema._htmlElementsClosingDT = new ArrayList();
            HtmlSchema._htmlElementsClosingDD.Add("dd");
            HtmlSchema._htmlElementsClosingDD.Add("dt");
            HtmlSchema._htmlElementsClosingLI = new ArrayList();
            HtmlSchema._htmlElementsClosingLI.Add("li");
            HtmlSchema._htmlElementsClosingTbody = new ArrayList();
            HtmlSchema._htmlElementsClosingTbody.Add("tbody");
            HtmlSchema._htmlElementsClosingTbody.Add("thead");
            HtmlSchema._htmlElementsClosingTbody.Add("tfoot");
            HtmlSchema._htmlElementsClosingTR = new ArrayList();
            HtmlSchema._htmlElementsClosingTR.Add("thead");
            HtmlSchema._htmlElementsClosingTR.Add("tfoot");
            HtmlSchema._htmlElementsClosingTR.Add("tbody");
            HtmlSchema._htmlElementsClosingTR.Add("tr");
            HtmlSchema._htmlElementsClosingTD = new ArrayList();
            HtmlSchema._htmlElementsClosingTD.Add("td");
            HtmlSchema._htmlElementsClosingTD.Add("th");
            HtmlSchema._htmlElementsClosingTD.Add("tr");
            HtmlSchema._htmlElementsClosingTD.Add("tbody");
            HtmlSchema._htmlElementsClosingTD.Add("tfoot");
            HtmlSchema._htmlElementsClosingTD.Add("thead");
            HtmlSchema._htmlElementsClosingTH = new ArrayList();
            HtmlSchema._htmlElementsClosingTH.Add("td");
            HtmlSchema._htmlElementsClosingTH.Add("th");
            HtmlSchema._htmlElementsClosingTH.Add("tr");
            HtmlSchema._htmlElementsClosingTH.Add("tbody");
            HtmlSchema._htmlElementsClosingTH.Add("tfoot");
            HtmlSchema._htmlElementsClosingTH.Add("thead");
            HtmlSchema._htmlElementsClosingThead = new ArrayList();
            HtmlSchema._htmlElementsClosingThead.Add("tbody");
            HtmlSchema._htmlElementsClosingThead.Add("tfoot");
            HtmlSchema._htmlElementsClosingTfoot = new ArrayList();
            HtmlSchema._htmlElementsClosingTfoot.Add("tbody");
            HtmlSchema._htmlElementsClosingTfoot.Add("thead");
        }
        private static void InitializeHtmlCharacterEntities()
        {
            HtmlSchema._htmlCharacterEntities = new Hashtable();
            HtmlSchema._htmlCharacterEntities["Aacute"] = 'Á';
            HtmlSchema._htmlCharacterEntities["aacute"] = 'á';
            HtmlSchema._htmlCharacterEntities["Acirc"] = 'Â';
            HtmlSchema._htmlCharacterEntities["acirc"] = 'â';
            HtmlSchema._htmlCharacterEntities["acute"] = '´';
            HtmlSchema._htmlCharacterEntities["AElig"] = 'Æ';
            HtmlSchema._htmlCharacterEntities["aelig"] = 'æ';
            HtmlSchema._htmlCharacterEntities["Agrave"] = 'À';
            HtmlSchema._htmlCharacterEntities["agrave"] = 'à';
            HtmlSchema._htmlCharacterEntities["alefsym"] = 'ℵ';
            HtmlSchema._htmlCharacterEntities["Alpha"] = 'Α';
            HtmlSchema._htmlCharacterEntities["alpha"] = 'α';
            HtmlSchema._htmlCharacterEntities["amp"] = '&';
            HtmlSchema._htmlCharacterEntities["and"] = '∧';
            HtmlSchema._htmlCharacterEntities["ang"] = '∠';
            HtmlSchema._htmlCharacterEntities["Aring"] = 'Å';
            HtmlSchema._htmlCharacterEntities["aring"] = 'å';
            HtmlSchema._htmlCharacterEntities["asymp"] = '≈';
            HtmlSchema._htmlCharacterEntities["Atilde"] = 'Ã';
            HtmlSchema._htmlCharacterEntities["atilde"] = 'ã';
            HtmlSchema._htmlCharacterEntities["Auml"] = 'Ä';
            HtmlSchema._htmlCharacterEntities["auml"] = 'ä';
            HtmlSchema._htmlCharacterEntities["bdquo"] = '„';
            HtmlSchema._htmlCharacterEntities["Beta"] = 'Β';
            HtmlSchema._htmlCharacterEntities["beta"] = 'β';
            HtmlSchema._htmlCharacterEntities["brvbar"] = '¦';
            HtmlSchema._htmlCharacterEntities["bull"] = '•';
            HtmlSchema._htmlCharacterEntities["cap"] = '∩';
            HtmlSchema._htmlCharacterEntities["Ccedil"] = 'Ç';
            HtmlSchema._htmlCharacterEntities["ccedil"] = 'ç';
            HtmlSchema._htmlCharacterEntities["cent"] = '¢';
            HtmlSchema._htmlCharacterEntities["Chi"] = 'Χ';
            HtmlSchema._htmlCharacterEntities["chi"] = 'χ';
            HtmlSchema._htmlCharacterEntities["circ"] = 'ˆ';
            HtmlSchema._htmlCharacterEntities["clubs"] = '♣';
            HtmlSchema._htmlCharacterEntities["cong"] = '≅';
            HtmlSchema._htmlCharacterEntities["copy"] = '©';
            HtmlSchema._htmlCharacterEntities["crarr"] = '↵';
            HtmlSchema._htmlCharacterEntities["cup"] = '∪';
            HtmlSchema._htmlCharacterEntities["curren"] = '¤';
            HtmlSchema._htmlCharacterEntities["dagger"] = '†';
            HtmlSchema._htmlCharacterEntities["Dagger"] = '‡';
            HtmlSchema._htmlCharacterEntities["darr"] = '↓';
            HtmlSchema._htmlCharacterEntities["dArr"] = '⇓';
            HtmlSchema._htmlCharacterEntities["deg"] = '°';
            HtmlSchema._htmlCharacterEntities["Delta"] = 'Δ';
            HtmlSchema._htmlCharacterEntities["delta"] = 'δ';
            HtmlSchema._htmlCharacterEntities["diams"] = '♦';
            HtmlSchema._htmlCharacterEntities["divide"] = '÷';
            HtmlSchema._htmlCharacterEntities["Eacute"] = 'É';
            HtmlSchema._htmlCharacterEntities["eacute"] = 'é';
            HtmlSchema._htmlCharacterEntities["Ecirc"] = 'Ê';
            HtmlSchema._htmlCharacterEntities["ecirc"] = 'ê';
            HtmlSchema._htmlCharacterEntities["Egrave"] = 'È';
            HtmlSchema._htmlCharacterEntities["egrave"] = 'è';
            HtmlSchema._htmlCharacterEntities["empty"] = '∅';
            HtmlSchema._htmlCharacterEntities["emsp"] = '\u2003';
            HtmlSchema._htmlCharacterEntities["ensp"] = '\u2002';
            HtmlSchema._htmlCharacterEntities["Epsilon"] = 'Ε';
            HtmlSchema._htmlCharacterEntities["epsilon"] = 'ε';
            HtmlSchema._htmlCharacterEntities["equiv"] = '≡';
            HtmlSchema._htmlCharacterEntities["Eta"] = 'Η';
            HtmlSchema._htmlCharacterEntities["eta"] = 'η';
            HtmlSchema._htmlCharacterEntities["ETH"] = 'Ð';
            HtmlSchema._htmlCharacterEntities["eth"] = 'ð';
            HtmlSchema._htmlCharacterEntities["Euml"] = 'Ë';
            HtmlSchema._htmlCharacterEntities["euml"] = 'ë';
            HtmlSchema._htmlCharacterEntities["euro"] = '€';
            HtmlSchema._htmlCharacterEntities["exist"] = '∃';
            HtmlSchema._htmlCharacterEntities["fnof"] = 'ƒ';
            HtmlSchema._htmlCharacterEntities["forall"] = '∀';
            HtmlSchema._htmlCharacterEntities["frac12"] = '½';
            HtmlSchema._htmlCharacterEntities["frac14"] = '¼';
            HtmlSchema._htmlCharacterEntities["frac34"] = '¾';
            HtmlSchema._htmlCharacterEntities["frasl"] = '⁄';
            HtmlSchema._htmlCharacterEntities["Gamma"] = 'Γ';
            HtmlSchema._htmlCharacterEntities["gamma"] = 'γ';
            HtmlSchema._htmlCharacterEntities["ge"] = '≥';
            HtmlSchema._htmlCharacterEntities["gt"] = '>';
            HtmlSchema._htmlCharacterEntities["harr"] = '↔';
            HtmlSchema._htmlCharacterEntities["hArr"] = '⇔';
            HtmlSchema._htmlCharacterEntities["hearts"] = '♥';
            HtmlSchema._htmlCharacterEntities["hellip"] = '…';
            HtmlSchema._htmlCharacterEntities["Iacute"] = 'Í';
            HtmlSchema._htmlCharacterEntities["iacute"] = 'í';
            HtmlSchema._htmlCharacterEntities["Icirc"] = 'Î';
            HtmlSchema._htmlCharacterEntities["icirc"] = 'î';
            HtmlSchema._htmlCharacterEntities["iexcl"] = '¡';
            HtmlSchema._htmlCharacterEntities["Igrave"] = 'Ì';
            HtmlSchema._htmlCharacterEntities["igrave"] = 'ì';
            HtmlSchema._htmlCharacterEntities["image"] = 'ℑ';
            HtmlSchema._htmlCharacterEntities["infin"] = '∞';
            HtmlSchema._htmlCharacterEntities["int"] = '∫';
            HtmlSchema._htmlCharacterEntities["Iota"] = 'Ι';
            HtmlSchema._htmlCharacterEntities["iota"] = 'ι';
            HtmlSchema._htmlCharacterEntities["iquest"] = '¿';
            HtmlSchema._htmlCharacterEntities["isin"] = '∈';
            HtmlSchema._htmlCharacterEntities["Iuml"] = 'Ï';
            HtmlSchema._htmlCharacterEntities["iuml"] = 'ï';
            HtmlSchema._htmlCharacterEntities["Kappa"] = 'Κ';
            HtmlSchema._htmlCharacterEntities["kappa"] = 'κ';
            HtmlSchema._htmlCharacterEntities["Lambda"] = 'Λ';
            HtmlSchema._htmlCharacterEntities["lambda"] = 'λ';
            HtmlSchema._htmlCharacterEntities["lang"] = '〈';
            HtmlSchema._htmlCharacterEntities["laquo"] = '«';
            HtmlSchema._htmlCharacterEntities["larr"] = '←';
            HtmlSchema._htmlCharacterEntities["lArr"] = '⇐';
            HtmlSchema._htmlCharacterEntities["lceil"] = '⌈';
            HtmlSchema._htmlCharacterEntities["ldquo"] = '“';
            HtmlSchema._htmlCharacterEntities["le"] = '≤';
            HtmlSchema._htmlCharacterEntities["lfloor"] = '⌊';
            HtmlSchema._htmlCharacterEntities["lowast"] = '∗';
            HtmlSchema._htmlCharacterEntities["loz"] = '◊';
            HtmlSchema._htmlCharacterEntities["lrm"] = '‎';
            HtmlSchema._htmlCharacterEntities["lsaquo"] = '‹';
            HtmlSchema._htmlCharacterEntities["lsquo"] = '‘';
            HtmlSchema._htmlCharacterEntities["lt"] = '<';
            HtmlSchema._htmlCharacterEntities["macr"] = '¯';
            HtmlSchema._htmlCharacterEntities["mdash"] = '—';
            HtmlSchema._htmlCharacterEntities["micro"] = 'µ';
            HtmlSchema._htmlCharacterEntities["middot"] = '·';
            HtmlSchema._htmlCharacterEntities["minus"] = '−';
            HtmlSchema._htmlCharacterEntities["Mu"] = 'Μ';
            HtmlSchema._htmlCharacterEntities["mu"] = 'μ';
            HtmlSchema._htmlCharacterEntities["nabla"] = '∇';
            HtmlSchema._htmlCharacterEntities["nbsp"] = '\u00a0';
            HtmlSchema._htmlCharacterEntities["ndash"] = '–';
            HtmlSchema._htmlCharacterEntities["ne"] = '≠';
            HtmlSchema._htmlCharacterEntities["ni"] = '∋';
            HtmlSchema._htmlCharacterEntities["not"] = '¬';
            HtmlSchema._htmlCharacterEntities["notin"] = '∉';
            HtmlSchema._htmlCharacterEntities["nsub"] = '⊄';
            HtmlSchema._htmlCharacterEntities["Ntilde"] = 'Ñ';
            HtmlSchema._htmlCharacterEntities["ntilde"] = 'ñ';
            HtmlSchema._htmlCharacterEntities["Nu"] = 'Ν';
            HtmlSchema._htmlCharacterEntities["nu"] = 'ν';
            HtmlSchema._htmlCharacterEntities["Oacute"] = 'Ó';
            HtmlSchema._htmlCharacterEntities["ocirc"] = 'ô';
            HtmlSchema._htmlCharacterEntities["OElig"] = 'Œ';
            HtmlSchema._htmlCharacterEntities["oelig"] = 'œ';
            HtmlSchema._htmlCharacterEntities["Ograve"] = 'Ò';
            HtmlSchema._htmlCharacterEntities["ograve"] = 'ò';
            HtmlSchema._htmlCharacterEntities["oline"] = '‾';
            HtmlSchema._htmlCharacterEntities["Omega"] = 'Ω';
            HtmlSchema._htmlCharacterEntities["omega"] = 'ω';
            HtmlSchema._htmlCharacterEntities["Omicron"] = 'Ο';
            HtmlSchema._htmlCharacterEntities["omicron"] = 'ο';
            HtmlSchema._htmlCharacterEntities["oplus"] = '⊕';
            HtmlSchema._htmlCharacterEntities["or"] = '∨';
            HtmlSchema._htmlCharacterEntities["ordf"] = 'ª';
            HtmlSchema._htmlCharacterEntities["ordm"] = 'º';
            HtmlSchema._htmlCharacterEntities["Oslash"] = 'Ø';
            HtmlSchema._htmlCharacterEntities["oslash"] = 'ø';
            HtmlSchema._htmlCharacterEntities["Otilde"] = 'Õ';
            HtmlSchema._htmlCharacterEntities["otilde"] = 'õ';
            HtmlSchema._htmlCharacterEntities["otimes"] = '⊗';
            HtmlSchema._htmlCharacterEntities["Ouml"] = 'Ö';
            HtmlSchema._htmlCharacterEntities["ouml"] = 'ö';
            HtmlSchema._htmlCharacterEntities["para"] = '¶';
            HtmlSchema._htmlCharacterEntities["part"] = '∂';
            HtmlSchema._htmlCharacterEntities["permil"] = '‰';
            HtmlSchema._htmlCharacterEntities["perp"] = '⊥';
            HtmlSchema._htmlCharacterEntities["Phi"] = 'Φ';
            HtmlSchema._htmlCharacterEntities["phi"] = 'φ';
            HtmlSchema._htmlCharacterEntities["pi"] = 'π';
            HtmlSchema._htmlCharacterEntities["piv"] = 'ϖ';
            HtmlSchema._htmlCharacterEntities["plusmn"] = '±';
            HtmlSchema._htmlCharacterEntities["pound"] = '£';
            HtmlSchema._htmlCharacterEntities["prime"] = '′';
            HtmlSchema._htmlCharacterEntities["Prime"] = '″';
            HtmlSchema._htmlCharacterEntities["prod"] = '∏';
            HtmlSchema._htmlCharacterEntities["prop"] = '∝';
            HtmlSchema._htmlCharacterEntities["Psi"] = 'Ψ';
            HtmlSchema._htmlCharacterEntities["psi"] = 'ψ';
            HtmlSchema._htmlCharacterEntities["quot"] = '"';
            HtmlSchema._htmlCharacterEntities["radic"] = '√';
            HtmlSchema._htmlCharacterEntities["rang"] = '〉';
            HtmlSchema._htmlCharacterEntities["raquo"] = '»';
            HtmlSchema._htmlCharacterEntities["rarr"] = '→';
            HtmlSchema._htmlCharacterEntities["rArr"] = '⇒';
            HtmlSchema._htmlCharacterEntities["rceil"] = '⌉';
            HtmlSchema._htmlCharacterEntities["rdquo"] = '”';
            HtmlSchema._htmlCharacterEntities["real"] = 'ℜ';
            HtmlSchema._htmlCharacterEntities["reg"] = '®';
            HtmlSchema._htmlCharacterEntities["rfloor"] = '⌋';
            HtmlSchema._htmlCharacterEntities["Rho"] = 'Ρ';
            HtmlSchema._htmlCharacterEntities["rho"] = 'ρ';
            HtmlSchema._htmlCharacterEntities["rlm"] = '‏';
            HtmlSchema._htmlCharacterEntities["rsaquo"] = '›';
            HtmlSchema._htmlCharacterEntities["rsquo"] = '’';
            HtmlSchema._htmlCharacterEntities["sbquo"] = '‚';
            HtmlSchema._htmlCharacterEntities["Scaron"] = 'Š';
            HtmlSchema._htmlCharacterEntities["scaron"] = 'š';
            HtmlSchema._htmlCharacterEntities["sdot"] = '⋅';
            HtmlSchema._htmlCharacterEntities["sect"] = '§';
            HtmlSchema._htmlCharacterEntities["shy"] = '­';
            HtmlSchema._htmlCharacterEntities["Sigma"] = 'Σ';
            HtmlSchema._htmlCharacterEntities["sigma"] = 'σ';
            HtmlSchema._htmlCharacterEntities["sigmaf"] = 'ς';
            HtmlSchema._htmlCharacterEntities["sim"] = '∼';
            HtmlSchema._htmlCharacterEntities["spades"] = '♠';
            HtmlSchema._htmlCharacterEntities["sub"] = '⊂';
            HtmlSchema._htmlCharacterEntities["sube"] = '⊆';
            HtmlSchema._htmlCharacterEntities["sum"] = '∑';
            HtmlSchema._htmlCharacterEntities["sup"] = '⊃';
            HtmlSchema._htmlCharacterEntities["sup1"] = '¹';
            HtmlSchema._htmlCharacterEntities["sup2"] = '²';
            HtmlSchema._htmlCharacterEntities["sup3"] = '³';
            HtmlSchema._htmlCharacterEntities["supe"] = '⊇';
            HtmlSchema._htmlCharacterEntities["szlig"] = 'ß';
            HtmlSchema._htmlCharacterEntities["Tau"] = 'Τ';
            HtmlSchema._htmlCharacterEntities["tau"] = 'τ';
            HtmlSchema._htmlCharacterEntities["there4"] = '∴';
            HtmlSchema._htmlCharacterEntities["Theta"] = 'Θ';
            HtmlSchema._htmlCharacterEntities["theta"] = 'θ';
            HtmlSchema._htmlCharacterEntities["thetasym"] = 'ϑ';
            HtmlSchema._htmlCharacterEntities["thinsp"] = '\u2009';
            HtmlSchema._htmlCharacterEntities["THORN"] = 'Þ';
            HtmlSchema._htmlCharacterEntities["thorn"] = 'þ';
            HtmlSchema._htmlCharacterEntities["tilde"] = '˜';
            HtmlSchema._htmlCharacterEntities["times"] = '×';
            HtmlSchema._htmlCharacterEntities["trade"] = '™';
            HtmlSchema._htmlCharacterEntities["Uacute"] = 'Ú';
            HtmlSchema._htmlCharacterEntities["uacute"] = 'ú';
            HtmlSchema._htmlCharacterEntities["uarr"] = '↑';
            HtmlSchema._htmlCharacterEntities["uArr"] = '⇑';
            HtmlSchema._htmlCharacterEntities["Ucirc"] = 'Û';
            HtmlSchema._htmlCharacterEntities["ucirc"] = 'û';
            HtmlSchema._htmlCharacterEntities["Ugrave"] = 'Ù';
            HtmlSchema._htmlCharacterEntities["ugrave"] = 'ù';
            HtmlSchema._htmlCharacterEntities["uml"] = '¨';
            HtmlSchema._htmlCharacterEntities["upsih"] = 'ϒ';
            HtmlSchema._htmlCharacterEntities["Upsilon"] = 'Υ';
            HtmlSchema._htmlCharacterEntities["upsilon"] = 'υ';
            HtmlSchema._htmlCharacterEntities["Uuml"] = 'Ü';
            HtmlSchema._htmlCharacterEntities["uuml"] = 'ü';
            HtmlSchema._htmlCharacterEntities["weierp"] = '℘';
            HtmlSchema._htmlCharacterEntities["Xi"] = 'Ξ';
            HtmlSchema._htmlCharacterEntities["xi"] = 'ξ';
            HtmlSchema._htmlCharacterEntities["Yacute"] = 'Ý';
            HtmlSchema._htmlCharacterEntities["yacute"] = 'ý';
            HtmlSchema._htmlCharacterEntities["yen"] = '¥';
            HtmlSchema._htmlCharacterEntities["Yuml"] = 'Ÿ';
            HtmlSchema._htmlCharacterEntities["yuml"] = 'ÿ';
            HtmlSchema._htmlCharacterEntities["Zeta"] = 'Ζ';
            HtmlSchema._htmlCharacterEntities["zeta"] = 'ζ';
            HtmlSchema._htmlCharacterEntities["zwj"] = '‍';
            HtmlSchema._htmlCharacterEntities["zwnj"] = '‌';
        }
    }
}
