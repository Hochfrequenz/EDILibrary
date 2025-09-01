namespace EDILibrary.Constants.German
{
    /// <summary>
    /// Prüfidentifikatoren
    /// Basierend auf https://www.edi-energy.de/ ➡ Dokumente ➡ Anwendungsübersicht der Prüfidentifikatoren 2.2 und 2.1
    /// Excel code generator snipet : =TEXTKETTE("public const string UTILMD_";E2;"_";WECHSELN(WECHSELN(GROSS2(WECHSELN(WECHSELN(WECHSELN(WECHSELN(KLEIN(F2);"ä";"ae");"ö";"oe");"ü";"ue");".";""));" ";"");"/";"_");" = """;E2;""";")
    /// </summary>
    public static class Pruefidentifikatoren
    {
#pragma warning disable CS1591

        #region APERAK

        public const string APERAK_92001_FehlermeldungSelbstDefiniert = "92001";

        #endregion

        #region CONTRL

        public const string CONTRL_91001_SyntaxFehlermeldungSelbstDefiniert = "91001";

        #endregion

        #region COMDIS

        public const string COMDIS_29001_AblehnungRemadv = "29001";
        public const string COMDIS_29002_AblehnungIftsta = "29002";

        #endregion

        #region IFTSTA

        public const string IFTSTA_21000_Mabis_StatusmeldungVomLfAnNb_Uenb = "21000";
        public const string IFTSTA_21001_Mabis_StatusmeldungVomNbAnNb = "21001";
        public const string IFTSTA_21002_Mabis_AbweisungVomBikoAnNb_Uenb = "21002";
        public const string IFTSTA_21003_Mabis_StatusmeldungVomBikoAnNb_Uenb = "21003";
        public const string IFTSTA_21004_Mabis_StatusmeldungVomBikoAnBkv_Nb = "21004";
        public const string IFTSTA_21005_Mabis_StatusmeldungVomBkv_NbAnBiko = "21005";
        public const string IFTSTA_21007_Wim_StatusmeldungPlanungszeitpunktVomNbAnMsba = "21007";
        public const string IFTSTA_21009_Wim_StatusmeldungGescheitertVomMsbnAnNb = "21009";
        public const string IFTSTA_21010_Wim_StatusmeldungErfolgreichVomMsbnAnNb = "21010";
        public const string IFTSTA_21011_Wim_StatusmeldungMsb_ScheitermeldungVomNbAnMsbn = "21011";
        public const string IFTSTA_21012_Wim_StatusmeldungErfolgreichVomNbAnMsbn = "21012";
        public const string IFTSTA_21013_Wim_StatusmeldungGescheitertVomNbAnMsbn = "21013";
        public const string IFTSTA_21015_Wim_InformationsmeldungErfolgreichVomNbAnMsba = "21015";
        public const string IFTSTA_21018_Wim_StatusmeldungVomNbAnMsba = "21018";
        public const string IFTSTA_21024_Wim_StatusmeldungErfolgreichVomMsbAnLf = "21024";
        public const string IFTSTA_21025_Wim_StatusmeldungGescheitertVomMsbAnLf = "21025";
        public const string IFTSTA_21026_Wim_StatusmeldungErfolgreichVomMsbAnNb = "21026";
        public const string IFTSTA_21027_Wim_StatusmeldungGescheitertVomMsbAnNb_Msb = "21027";
        public const string IFTSTA_21028_Wim_InformationsmeldungVomMsbAnNb = "21028";
        public const string IFTSTA_21029_Wim_Vorabinformation = "21029";
        public const string IFTSTA_21030_Wim_ZustimmungImsErsteinbau = "21030";
        public const string IFTSTA_21031_Wim_BestandsschutzEigenausbauIms = "21031";
        public const string IFTSTA_21032_Wim_Angebotsablehn0ungLfAnMsb = "21032";
        public const string IFTSTA_21033_Wim_Gpke_AnfrageablehnungMsbAnEsa_Lf_Nb = "21033";
        public const string IFTSTA_21035_Gpke_RueckmeldungAufLieferschein = "21035";
        public const string IFTSTA_21036_Wim_Geraetestatus = "21036";
        public const string IFTSTA_21037_Rd_NbBewertung = "21037";
        public const string IFTSTA_21038_Rd_BtrBewertung = "21038";
        public const string IFTSTA_21039_Gpke_AwhSperrpGas_AuftragsstatusSperren = "21039";
        public const string IFTSTA_21040_Gpke_AwhSperrpGas_InformationUeberEntsperrauftrag =
            "21040";
        public const string IFTSTA_21042_Wim_UmsetzungsstatusDerBestellung = "21042";
        public const string IFTSTA_21043_Gpke_BestellungsantwortMitteilung = "21043";
        public const string IFTSTA_21044_Gpke_Bestellungsbeendigung = "21044";
        public const string IFTSTA_21047_Gpke_Bearbeitungsstandsmeldung = "21047";

        #endregion

        #region INSRPT

        public const string INSRPT_23001_Wim_StoerungsmeldungVomLf_Nb_MsbAnMsb = "23001";
        public const string INSRPT_23003_Wim_AblehnungVomMsbAnLf_Nb_Msb = "23003";
        public const string INSRPT_23004_Wim_BestaetigungVomMsbAnLf_Nb_Msb = "23004";
        public const string INSRPT_23005_Wim_InformationsmeldungStoerungsmeldungVomMsbAnNbGas_MsbStrom =
            "23005";
        public const string INSRPT_23008_Wim_ErgebnisberichtVomMsbAnLf_Nb_Msb = "23008";
        public const string INSRPT_23009_Wim_InformationsmeldungStoerungsbehebungVomMsbAnNbGas_MsbStrom =
            "23009";
        public const string INSRPT_23011_Wim_InformationsmeldungStoerungsmeldungVomMsbDerMalo =
            "23011";
        public const string INSRPT_23012_Wim_InformationsmeldungStoerungsbehebungVomMsbDerMalo =
            "23012";

        #endregion

        #region INVOIC

        public const string INVOIC_31001_Abschlagsrechnung = "31001";
        public const string INVOIC_31002_NnRechnung = "31002";
        public const string INVOIC_31003_WimRechnung = "31003";
        public const string INVOIC_31004_Stornorechnung = "31004";
        public const string INVOIC_31005_MmmRechnung = "31005";
        public const string INVOIC_31006_MmmSelbstAusgestellteRechnung = "31006";
        public const string INVOIC_31007_AggregierteMmmRechnung = "31007";
        public const string INVOIC_31008_AggregierteMmmSelbstAusgestellteRechnung = "31008";
        public const string INVOIC_31009_MsbRechnung = "31009";
        public const string INVOIC_31010_Kapazitaetsrechnung = "31010";
        public const string INVOIC_31011_RechnungSonstigeLeistung = "31011";

        #endregion


        #region MSCONS

        public const string MSCONS_13002_MesswZaehlerstandGas = "13002";
        public const string MSCONS_13003_Summenzeitreihe = "13003";
        public const string MSCONS_13005_EegUeberfzr = "13005";
        public const string MSCONS_13006_MesswStorno = "13006";
        public const string MSCONS_13007_Gasbeschaffenheitsdaten = "13007";
        public const string MSCONS_13008_MesswertLastgangGas = "13008";
        public const string MSCONS_13009_MesswertEnergiemengeGas = "13009";
        public const string MSCONS_13010_Profil = "13010";
        public const string MSCONS_13011_Profilschar = "13011";
        public const string MSCONS_13012_TepVergangenheitswerteReferenzMessung = "13012";
        public const string MSCONS_13013_MarktlokationsscharfeAllokationslisteGasMmma = "13013";
        public const string MSCONS_13014_MarktlokationsscharfeBilanzierteMengeMmma = "13014";
        public const string MSCONS_13015_BewegungsdatenImKalenderjahrVorLieferbeginn = "13015";
        public const string MSCONS_13016_EnergiemengeUndLeistungsmaximum = "13016";
        public const string MSCONS_13017_MesswZaehlerstandStrom = "13017";
        public const string MSCONS_13018_LastgangMesslokationNetzkoppelpunkt = "13018";
        public const string MSCONS_13019_MesswertEnergiemengeStrom = "13019";
        public const string MSCONS_13020_RedispatchAusfallarbeitsueberfuehrungszeitreihe = "13020";
        public const string MSCONS_13021_RedispatchUebermittlungVonMeteorologischenDaten = "13021";
        public const string MSCONS_13022_RedispatchEinzelzeitreiheAusfallarbeit = "13022";
        public const string MSCONS_13023_RedispatchAusfallarbeitssummenzeitreihe = "13023";
        public const string MSCONS_13025_LastgangMarktlokationTranche = "13025";
        public const string MSCONS_13026_RedispatchEegUeberfuehrungszeitreiheAufgrundAusfallarbeit =
            "13026";
        public const string MSCONS_13027_WerteNachTyp2 = "13027";

        #endregion

        #region ORDCHG

        public const string ORDCHG_39000_StornierungSperrEntsperrauftrag = "39000";
        public const string ORDCHG_39001_WeiterleitungDerStornierung = "39001";
        public const string ORDCHG_39002_StornierungDerBestellungFuerEsa = "39002";

        #endregion

        #region ORDERS

        public const string ORDERS_17001_BestellungGeraeteuebernahme = "17001";
        public const string ORDERS_17002_WeiterverpflichtungMsba = "17002";
        public const string ORDERS_17003_BeauftragungZurAenderungDerMesslokation = "17003";
        public const string ORDERS_17004_AnforderungMesswerte = "17004";
        public const string ORDERS_17005_BestellungRechnungsabwicklungMsbUeberLf = "17005";
        public const string ORDERS_17006_BeendigungRechnungsabwicklungMsbUeberLf = "17006";
        public const string ORDERS_17007_BestellungVonWertenFuerEsa = "17007";
        public const string ORDERS_17008_AbbestellungVonWertenFuerEsa = "17008";
        public const string ORDERS_17009_AnzeigeGeraetewechselabsicht = "17009";
        public const string ORDERS_17011_BestellungAngebotÄnderungTechnik = "17011";
        public const string ORDERS_17101_AnfrageStammdatenMarktlokation = "17101";
        public const string ORDERS_17102_AnfrageWerte = "17102";
        public const string ORDERS_17103_AnfrageBrennwertUndZustandszahl = "17103";
        public const string ORDERS_17104_AnfrageVomMsbGas = "17104";
        public const string ORDERS_17110_AnforderungAllokationsliste = "17110";
        public const string ORDERS_17113_MeldungEinerReklamationVonWerten = "17113";
        public const string ORDERS_17114_AnforderungBilanzierteMenge = "17114";
        public const string ORDERS_17115_Sperrauftrag = "17115";
        public const string ORDERS_17116_AnfrageSperrung = "17116";
        public const string ORDERS_17117_Entsperrauftrag = "17117";
        public const string ORDERS_17118_BestellungKonfigurationsaenderung = "17118";
        public const string ORDERS_17119_BestellungAenderungKonzessionsabgabe = "17119";
        public const string ORDERS_17120_BestellungAenderungPrognosegrundlage = "17120";
        public const string ORDERS_17121_BestellungAenderung = "17121";
        public const string ORDERS_17122_ReklamationEinerDefinition = "17122";
        public const string ORDERS_17123_BestellungAenderungZaehlzeitdefinition = "17123";
        public const string ORDERS_17126_AnfrageStammdatenMesslokation = "17126";
        public const string ORDERS_17127_AnfrageStammdatenTranche = "17127";
        public const string ORDERS_17128_ReklamationEinerKonfiguration = "17128";
        public const string ORDERS_17129_BestellungBeendigungEinerKonfiguration = "17129";
        public const string ORDERS_17130_BestellungAenderungEinerKonfiguration = "17130";
        public const string ORDERS_17131_BestellungEinesAngebotsEinerKonfiguration = "17131";
        public const string ORDERS_17133_BestellungAenderungAbrechnungsdaten = "17133";
        public const string ORDERS_17201_AnforderungNormierterProfile_Profilscharen = "17201";
        public const string ORDERS_17202_AnforderungLieferantenclearingliste = "17202";
        public const string ORDERS_17203_AnforderungBilanzkreiszuordnungsliste = "17203";
        public const string ORDERS_17204_AnforderungClearinglisteBas = "17204";
        public const string ORDERS_17205_AnforderungClearinglisteDzr = "17205";
        public const string ORDERS_17206_AnforderungBilanzierungsgebietsclearingliste = "17206";
        public const string ORDERS_17207_AbBestellungBkSzrAufAggregationsebeneRz = "17207";
        public const string ORDERS_17208_AnforderungClearinglisteUenbDzr = "17208";
        public const string ORDERS_17209_AnforderungAusfallarbeit = "17209";
        public const string ORDERS_17210_Anforderung = "17210";
        public const string ORDERS_17211_ReklamationProfileBzwProfilscharen = "17211";
        public const string ORDERS_17301_AnforderungAbo = "17301";
        public const string ORDERS_17132_Gpke_AnfrageStammdatenStrom = "17132";
        public const string ORDERS_17133_Gpke_BestellungAenderungAbrechnungsdaten = "17133";
        public const string ORDERS_17134_Gpke_EinrichtungKonfigurationZuordnungLfVonNb = "17134";
        public const string ORDERS_17135_Gpke_EinrichtungKonfigurationZuordnungLfVonMsb = "17135";

        #endregion

        #region ORDRSP

        public const string ORDRSP_19001_Bestellbestaetigung = "19001";
        public const string ORDRSP_19002_AblehnungDerBestellung = "19002";
        public const string ORDRSP_19003_FortfuehrungsbestaetigungMsba = "19003";
        public const string ORDRSP_19004_AblehnungFortfuehrung = "19004";
        public const string ORDRSP_19005_AuftragsbestaetigungDerAenderungDerMesslokation = "19005";
        public const string ORDRSP_19006_AblehnungDerAenderungDerMesslokation = "19006";
        public const string ORDRSP_19007_AblehnungAnforderungMesswerte = "19007";
        public const string ORDRSP_19009_BestaetigungBeendigungRechnungsabwicklungMsbUeberLf =
            "19009";
        public const string ORDRSP_19010_AblehnungBeendigungRechnungsabwicklungMsbUeberLf = "19010";
        public const string ORDRSP_19011_BestaetigungDerAbBestellungVonWertenFuerEsa = "19011";
        public const string ORDRSP_19012_AblehnungDerAbBestellungVonWertenFuerEsa = "19012";
        public const string ORDRSP_19013_BestaetigungDerStornierungEinerBestellungFuerEsa = "19013";
        public const string ORDRSP_19014_AblehnungDerStornierungEinerBestellungFuerEsa = "19014";
        public const string ORDRSP_19015_BestaetigungGeraetewechselabsicht = "19015";
        public const string ORDRSP_19016_AblehnungGeraetewechselabsicht = "19016";
        public const string ORDRSP_19101_AblehnungAnfrageStammdaten = "19101";
        public const string ORDRSP_19102_AblehnungAnfrageWerte = "19102";
        public const string ORDRSP_19103_AblehnungBrennwertUndZustandszahl = "19103";
        public const string ORDRSP_19104_AblehnungAnfrageVomMsbGas = "19104";
        public const string ORDRSP_19110_AblehnungAllokationsliste = "19110";
        public const string ORDRSP_19114_AblehnungDerReklamationVonWerten = "19114";
        public const string ORDRSP_19115_AblehnungAnforderungBilanzierteMenge = "19115";
        public const string ORDRSP_19116_BestaetigungSperrEntsperrauftrag = "19116";
        public const string ORDRSP_19117_AblehnungSperrEntsperrauftrag = "19117";
        public const string ORDRSP_19118_BestaetigungAnfrageSperrung = "19118";
        public const string ORDRSP_19119_AblehnungAnfrageSperrung = "19119";
        public const string ORDRSP_19120_MitteilungZurAenderung = "19120";
        public const string ORDRSP_19121_MitteilungZurAenderungPrognosegrundlage = "19121";
        public const string ORDRSP_19122_AblehnungAenderungKonzessionsabgabe = "19122";
        public const string ORDRSP_19123_AblehnungDerReklamationEinerDefinition = "19123";
        public const string ORDRSP_19124_MitteilungZurAenderungZaehlzeitdefinition = "19124";
        public const string ORDRSP_19127_MitteilungZurKonfigurationsaenderung = "19127";
        public const string ORDRSP_19128_BestaetigungStornierungSperrEntsperrauftrag = "19128";
        public const string ORDRSP_19129_AblehnungStornierungSperrEntsperrauftrag = "19129";
        public const string ORDRSP_19130_BearbeitungsstandZurReklamationEiner = "19130";
        public const string ORDRSP_19131_AntwortAufBestellungBeendigungEinerKonfiguration = "19131";
        public const string ORDRSP_19132_MitteilungZurBestellungAenderungEinerKonfiguration =
            "19132";
        public const string ORDRSP_19133_BearbeitungsstandBestellungAenderungAbrechnungsdaten =
            "19133";
        public const string ORDRSP_19204_AblehnungAbBestellungDerAggregationsebene = "19204";
        public const string ORDRSP_19301_AblehnungAbo = "19301";
        public const string ORDRSP_19302_BestaetigungEndeAbo = "19302";
        public const string ORDRSP_19133_Gpke_BearbeitungsstandBestellungAenderungAbrechnungsdaten =
            "19133";

        #endregion


        #region PARTIN

        public const string PARTIN_37000_UebermittlungKommunikationsdatenDesLf = "37000";
        public const string PARTIN_37001_UebermittlungKommunikationsdatenDesNb = "37001";
        public const string PARTIN_37002_UebermittlungKommunikationsdatenDesMsb = "37002";
        public const string PARTIN_37003_UebermittlungKommunikationsdatenDesBkv = "37003";
        public const string PARTIN_37004_UebermittlungKommunikationsdatenDesBiko = "37004";
        public const string PARTIN_37005_UebermittlungKommunikationsdatenDesUenb = "37005";
        public const string PARTIN_37006_UebermittlungKommunikationsdatenDesEsa = "37006";
        public const string PARTIN_37007_UebermittlungKommunikationsparameter = "37007";

        #endregion

        #region PRICAT

        public const string PRICAT_27001_UebermittlungDerAusgleichsenergiepreise = "27001";
        public const string PRICAT_27002_PreisblattMsbLeistungen = "27002";
        public const string PRICAT_27003_PreisblattNbLeistungen = "27003";

        #endregion

        #region QUOTES

        public const string QUOTES_15001_AngebotGeraeteuebernahme = "15001";
        public const string QUOTES_15002_AngebotRechnungsabwicklungMsbUeberLf = "15002";
        public const string QUOTES_15003_AngebotZurAnfrageVonWertenFuerEsa = "15003";
        public const string QUOTES_15004_AngebotZurAnfrageEinerKonfiguration = "15004";

        #endregion

        #region REMADV

        public const string REMADV_33001_Bestaetigung = "33001";
        public const string REMADV_33002_Abweisung = "33002";
        public const string REMADV_33003_AbweisungKopfUndSumme = "33003";
        public const string REMADV_33004_AbweisungPosition = "33004";

        #endregion

        #region REQUOTE

        public const string REQOTE_35001_AnforderungAngebotGeraeteuebernahme = "35001";
        public const string REQOTE_35002_AnfrageRechnungsabwicklungMsbUeberLf = "35002";
        public const string REQOTE_35003_AnfrageVonWertenFuerEsa = "35003";
        public const string REQOTE_35004_AnfrageEinerKonfiguration = "35004";
        public const string REQOTE_35005_AnfrageAngebotAenderungTechnik = "35005";

        #endregion

        #region UTILTS

        public const string UTILTS_25001_Berechnungsformel = "25001";
        public const string UTILTS_25002_AblehnungBerechnungsformel = "25002";
        public const string UTILTS_25003_ZustimmungBerechnungsformel = "25003";
        public const string UTILTS_25004_UebersichtZaehlzeitdefinitionen = "25004";
        public const string UTILTS_25005_AusgerollteZaehlzeitdefinition = "25005";
        public const string UTILTS_25006_UebersichtSchaltzeitdefinitionen = "25006";
        public const string UTILTS_25007_UebersichtLeistungskurvendefinitionen = "25007";
        public const string UTILTS_25008_AusgerollteSchaltzeitdefinition = "25008";
        public const string UTILTS_25009_AusgerollteLeistungskurvendefinition = "25009";
        public const string UTILTS_25010_WimStrom_AntwortAufBerechnungsformel = "25010";

        #endregion

        #region UTILMD

        public const string UTILMD_11001_Gpke_GeliGas_AnmeldungNn = "11001";
        public const string UTILMD_11002_Gpke_GeliGas_BestaetigungAnmeldung = "11002";
        public const string UTILMD_11003_Gpke_GeliGas_AblehnungAnmeldung = "11003";
        public const string UTILMD_11004_Gpke_GeliGas_AbmeldungNn = "11004";
        public const string UTILMD_11005_Gpke_GeliGas_BestaetigungAbmeldung = "11005";
        public const string UTILMD_11006_Gpke_GeliGas_AblehnungAbmeldung = "11006";
        public const string UTILMD_11007_Gpke_GeliGas_AbmeldungNnVomNb = "11007";
        public const string UTILMD_11008_Gpke_GeliGas_BestaetigungAbmeldungVomNb = "11008";
        public const string UTILMD_11009_Gpke_GeliGas_AblehnungAbmeldungVomNb = "11009";
        public const string UTILMD_11010_Gpke_GeliGas_AbmeldeanfrageDesNb = "11010";
        public const string UTILMD_11011_Gpke_GeliGas_BestaetigungAbmeldeanfrage = "11011";
        public const string UTILMD_11012_Gpke_GeliGas_AblehnungAbmeldeanfrage = "11012";
        public const string UTILMD_11013_Gpke_GeliGas_AnmeldungEog = "11013";
        public const string UTILMD_11014_Gpke_GeliGas_BestaetigungEogAnmeldung = "11014";
        public const string UTILMD_11015_Gpke_GeliGas_AblehnungEogAnmeldung = "11015";
        public const string UTILMD_11016_Gpke_GeliGas_KuendigungBeimAltenLieferanten = "11016";
        public const string UTILMD_11017_Gpke_GeliGas_BestaetigungKuendigung = "11017";
        public const string UTILMD_11018_Gpke_GeliGas_AblehnungKuendigung = "11018";
        public const string UTILMD_11019_GeliGas_ZuordnungslisteZugeordneteMarktlokationen =
            "11019";
        public const string UTILMD_11020_GeliGas_AenderungsmeldungZurZuordnungsliste = "11020";
        public const string UTILMD_11021_GeliGas_AntwortAufAenderungsmeldungZurZuordnungsliste =
            "11021";
        public const string UTILMD_11022_Gpke_GeliGas_AnfrageNachStornierung = "11022";
        public const string UTILMD_11023_Gpke_GeliGas_BestaetigungAnfrageStornierung = "11023";
        public const string UTILMD_11024_Gpke_GeliGas_AblehnungAnfrageStornierung = "11024";
        public const string UTILMD_11035_Gpke_GeliGas_AntwortAufDieGeschaeftsdatenanfrage = "11035";
        public const string UTILMD_11036_Gpke_GeliGas_InformationsmeldungUeberExistierendeZuordnung =
            "11036";
        public const string UTILMD_11037_Gpke_GeliGas_InformationsmeldungZurBeendigungDerZuordnung =
            "11037";
        public const string UTILMD_11038_Gpke_GeliGas_InformationsmeldungZurAufhebungEinerZukZuordnung =
            "11038";
        public const string UTILMD_11039_Wim_KuendigungMsb = "11039";
        public const string UTILMD_11040_Wim_BestaetigungKuendigungMsb = "11040";
        public const string UTILMD_11041_Wim_AblehnungKuendigungMsb = "11041";
        public const string UTILMD_11042_Wim_AnmeldungMsb = "11042";
        public const string UTILMD_11043_Wim_BestaetigungAnmeldungMsb = "11043";
        public const string UTILMD_11044_Wim_AblehnungAnmeldungMsb = "11044";
        public const string UTILMD_11051_Wim_EndeMsb = "11051";
        public const string UTILMD_11052_Wim_BestaetigungEndeMsb = "11052";
        public const string UTILMD_11053_Wim_AblehnungEndeMsb = "11053";
        public const string UTILMD_11060_Wim_AntwortAufDieMsbGeschaeftsdatenanfrage = "11060";
        public const string UTILMD_11062_Mabis_AktivierungVonMabisZp = "11062";
        public const string UTILMD_11063_Mabis_DeaktivierungVonMabisZp = "11063";
        public const string UTILMD_11064_Mabis_Antwort = "11064";
        public const string UTILMD_11065_Mabis_Lieferantenclearingliste = "11065";
        public const string UTILMD_11066_Mabis_KorrekturlisteZuLieferantenclearingliste = "11066";
        public const string UTILMD_11067_Mabis_Bilanzkreiszuordnungsliste = "11067";
        public const string UTILMD_11069_Mabis_ClearinglisteDzr = "11069";
        public const string UTILMD_11070_Mabis_ClearinglisteBas = "11070";
        public const string UTILMD_11071_Mabis_AktivierungDerZuordnungsermaechtigung = "11071";
        public const string UTILMD_11072_Mabis_DeaktivierungDerZuordnungsermaechtigung = "11072";
        public const string UTILMD_11073_Mabis_UebermittlungDerProfildefinitionen = "11073";
        public const string UTILMD_11074_Hknr_StammdatenAufEineOrders = "11074";
        public const string UTILMD_11075_Hknr_StammdatenAufgrundEinerAenderung = "11075";
        public const string UTILMD_11076_Hknr_AntwortAufStammdatenaenderung = "11076";
        public const string UTILMD_11077_Mpes_Anmeldung = "11077";
        public const string UTILMD_11078_Mpes_BestaetigungAnmeldung = "11078";
        public const string UTILMD_11079_Mpes_BestaetigungAnmeldungNeuanlageUndBestaetigungLf =
            "11079";
        public const string UTILMD_11080_Mpes_AblehnungAnmeldung = "11080";
        public const string UTILMD_11081_Mpes_Abmeldung = "11081";
        public const string UTILMD_11082_Mpes_BestaetigungAbmeldung = "11082";
        public const string UTILMD_11083_Mpes_AblehnungAbmeldung = "11083";
        public const string UTILMD_11084_Mpes_AbmeldungStilllegung = "11084";
        public const string UTILMD_11085_Mpes_BestaetigungAbmeldungStilllegung = "11085";
        public const string UTILMD_11086_Mpes_AbmeldeanfrageDesNb = "11086";
        public const string UTILMD_11087_Mpes_BestaetigungAbmeldeanfrage = "11087";
        public const string UTILMD_11088_Mpes_AblehnungAbmeldeanfrage = "11088";
        public const string UTILMD_11089_Mpes_KuendigungBeimAltenLieferanten = "11089";
        public const string UTILMD_11090_Mpes_BestaetigungKuendigung = "11090";
        public const string UTILMD_11091_Mpes_AblehnungKuendigung = "11091";
        public const string UTILMD_11092_Mpes_InformationsmeldungUeberExistierendeZuordnung =
            "11092";
        public const string UTILMD_11093_Mpes_InformationsmeldungZurBeendigungDerZuordnung =
            "11093";
        public const string UTILMD_11094_Mpes_InformationsmeldungZurAufhebungEinerZukZuordnung =
            "11094";
        public const string UTILMD_11095_Mpes_AntwortAufDieGeschaeftsdatenanfrage = "11095";
        public const string UTILMD_11096_Tsimsg_DeklarationslisteAnMgv = "11096";
        public const string UTILMD_11097_Tsimsg_DeklarationslisteAnBkv = "11097";
        public const string UTILMD_11101_Nbw_StammdatenZurMesslokation = "11101";
        public const string UTILMD_11102_Nbw_AktualisierteStammdatenZurMesslokation = "11102";
        public const string UTILMD_11103_Nbw_StammdatenZurVerbrauchendenMarktlokation = "11103";
        public const string UTILMD_11104_Nbw_AktualisierteStammdatenZurVerbrauchendenMarktlokation =
            "11104";
        public const string UTILMD_11105_Nbw_AblehnungAufStammdatenZurVerbrauchendenMarktlokation =
            "11105";
        public const string UTILMD_11106_Nbw_StammdatenZurErzeugendenMarktlokation = "11106";
        public const string UTILMD_11107_Nbw_AktualisierteStammdatenZurErzeugendenMarktlokation =
            "11107";
        public const string UTILMD_11108_Nbw_AblehnungAufStammdatenZurErzeugendenMarktlokation =
            "11108";
        public const string UTILMD_11109_Sdae_NichtBilarelAenderungVomLfAnNb = "11109";
        public const string UTILMD_11110_Sdae_NichtBilarelAenderungVomLfNbAnMsb = "11110";
        public const string UTILMD_11111_Sdae_AntwortAufAenderungVomLfAgAnAf = "11111";
        public const string UTILMD_11112_Sdae_NichtBilarelAenderungVomNbAnLf = "11112";
        public const string UTILMD_11113_Sdae_NichtBilarelAenderungVomNbAnMsb = "11113";
        public const string UTILMD_11115_Sdae_AntwortAufAenderungVomNbAgAnNb = "11115";
        public const string UTILMD_11116_Sdae_AenderungVomMsbAnNb = "11116";
        public const string UTILMD_11117_Sdae_AenderungVomMsbNbAnLf = "11117";
        public const string UTILMD_11119_Sdae_AntwortAufAenderungVomMsbAgAnAf = "11119";
        public const string UTILMD_11120_Sdae_BilarelAenderungVomLfAnNb = "11120";
        public const string UTILMD_11121_Sdae_AntwortAufBilarelAenderungVomLfNbAnLf = "11121";
        public const string UTILMD_11123_Sdae_BilarelAenderungVomNbMitAbhaengigkeitenNbAnLf_Msb =
            "11123";
        public const string UTILMD_11124_Sdae_AntwortAufBilarelAenderungVomNbMitAbhaengigkeitenAgAnNb =
            "11124";
        public const string UTILMD_11126_Sdae_AenderungDerPrognosegrundlageNbAnLf_Msb = "11126";
        public const string UTILMD_11127_Sdae_AntwortAufAenderungDerPrognosegrundlageAgAnNb =
            "11127";
        public const string UTILMD_11133_Sdae_NichtBilarelAnfrageAnLfMsbAnNb = "11133";
        public const string UTILMD_11134_Sdae_AntwortAufNichtBilarelAnfrageAnLfNbAnMsb = "11134";
        public const string UTILMD_11135_Sdae_AntwortAufNichtBilarelAnfrageAnLfNbAnMsb = "11135";
        public const string UTILMD_11136_Sdae_NichtBilarelAnfrageAnLfNbVerteilerAnLf = "11136";
        public const string UTILMD_11137_Sdae_NichtBilarelAnfrageAnLfNbBerechtigtAnLf = "11137";
        public const string UTILMD_11138_Sdae_AntwortAufNichtBilarelAnfrageAnLfLfAnNb = "11138";
        public const string UTILMD_11139_Sdae_NichtBilarelAnfrageAnNbLfAnNb = "11139";
        public const string UTILMD_11140_Sdae_NichtBilarelAnfrageAnNbMsbAnNb = "11140";
        public const string UTILMD_11142_Sdae_AntwortAufNichtBilarelAnfrageAnNbNbAnAf = "11142";
        public const string UTILMD_11143_Sdae_AnfrageAnMsbMitAbhaengigkeitenLfAnNb = "11143";
        public const string UTILMD_11145_Sdae_AntwortAufAnfrageAnMsbMitAbhaengigkeitenNbAnAf =
            "11145";
        public const string UTILMD_11146_Sdae_AblehnungDerAnfrageAnMsbMitAbhaengigkeitenNbAnAf =
            "11146";
        public const string UTILMD_11147_Sdae_AnfrageAnMsbMitAbhaengigkeitenNbVerteilerAnMsb =
            "11147";
        public const string UTILMD_11148_Sdae_AnfrageAnMsbMitAbhaengigkeitenNbBerechtigtAnMsb =
            "11148";
        public const string UTILMD_11149_Sdae_AntwortAufAnfrageAnMsbMitAbhaengigkeitenMsbAnNb =
            "11149";
        public const string UTILMD_11150_Sdae_BilarelAnfrageAnLfNbAnLf = "11150";
        public const string UTILMD_11151_Sdae_AntwortAufBilarelAnfrageAnLfLfAnNb = "11151";
        public const string UTILMD_11152_Sdae_AblehnungDerBilarelAnfrageAnLfLfAnNb = "11152";
        public const string UTILMD_11153_Sdae_BilarelAnfrageAnNbOhneAbhaengigkeiten = "11153";
        public const string UTILMD_11154_Sdae_AntwortAufBilarelAnfrageAnNbOhneAbhaengigkeiten =
            "11154";
        public const string UTILMD_11155_Sdae_AblehnungDerBilarelAnfrageAnNbOhneAbhaengigkeiten =
            "11155";
        public const string UTILMD_11156_Sdae_BilarelAnfrageAnNbMitAbhaengigkeiten = "11156";
        public const string UTILMD_11157_Sdae_AntwortAufBilarelAnfrageAnNbMitAbhaengigkeiten =
            "11157";
        public const string UTILMD_11159_Sdae_AenderungVomMsbOhneAbhaengigkeitenMsbAnNb = "11159";
        public const string UTILMD_11160_Sdae_AenderungVomMsbOhneAbhaengigkeitenNbAnLf_Msb =
            "11160";
        public const string UTILMD_11161_Sdae_AenderungVomMsbOhneAbhaengigkeitenAgAnAf = "11161";
        public const string UTILMD_11162_Sdae_AnfrageAnMsbOhneAbhaengigkeitenVonLf_MsbAnNb =
            "11162";
        public const string UTILMD_11163_Sdae_AntwortAufAnfrageAnMsbOhneAbhaengigkeitenLf_MsbAnNb =
            "11163";
        public const string UTILMD_11164_Sdae_AblehnungAufAnfrageAnMsbOhneAbhaengigkeitenLf_MsbAnNb =
            "11164";
        public const string UTILMD_11165_Sdae_AnfrageAnMsbOhneAbhaengigkeitenNbVerteilerAnMsb =
            "11165";
        public const string UTILMD_11166_Sdae_AnfrageAnMsbOhneAbhaengigkeitenNbBerechtigterAnMsb =
            "11166";
        public const string UTILMD_11167_Sdae_AntwortAufAnfrageAnMsbOhneAbhaengigkeitenMsbAnNb =
            "11167";
        public const string UTILMD_11168_Verpflichtungsanfrage_Aufforderung = "11168";
        public const string UTILMD_11169_BestaetigungVerpflichtungsanfrage = "11169";
        public const string UTILMD_11170_AblehnungVerpflichtungsanfrage = "11170";
        public const string UTILMD_11171_Sdae_AenderungVomMsbNbAnMsb = "11171";
        public const string UTILMD_11172_Sdae_AnfrageAnMsbMitAbhaengigkeitMsbAnNb = "11172";
        public const string UTILMD_11173_Sdae_AenderungDerLokationsbuendelstrukturNbAnMsb = "11173";
        public const string UTILMD_11174_Sdae_AntwortAufAenderungDerLokationsbuendelstrukturMsbAnNb =
            "11174";
        public const string UTILMD_11175_Sdae_AenderungDerMarktlokationsstrukturNbAnLf = "11175";
        public const string UTILMD_11176_Sdae_AntwortAufAenderungDerMarktlokationsstrukturLfAnNb =
            "11176";
        public const string UTILMD_11177_Sdae_AnfrageDerLokationsbuendelstrukturMsbAnNb = "11177";
        public const string UTILMD_11178_Sdae_AntwortAufAnfrageDerLokationsbuendelstrukturNbAnMsb =
            "11178";
        public const string UTILMD_11180_Sdae_AnfrageDerKomplexenMarktlokationsstrukturLfAnNb =
            "11180";
        public const string UTILMD_11181_Sdae_AntwortAufAnfrageDerKomplexenMarktlokationsstrukturNbAnLf =
            "11181";
        public const string UTILMD_11182_Sdae_AblehnungDerAnfrageDerKomplexenMarktlokationsstrukturNbAnLf =
            "11182";
        public const string UTILMD_11185_StammdatensynchronisationVomNb = "11185";
        public const string UTILMD_11186_StammdatensynchronisationVomLf = "11186";
        public const string UTILMD_11187_StammdatensynchronisationVomUenb = "11187";
        public const string UTILMD_11188_BeendigungDerAggregationsverantwortungVomNb = "11188";
        public const string UTILMD_11189_BeendigungDerAggregationsverantwortungVomLf = "11189";
        public const string UTILMD_11190_BeendigungDerAggregationsverantwortungVomUenb = "11190";
        public const string UTILMD_11191_AnfrageAnNbVomUenb = "11191";
        public const string UTILMD_11192_AntwortAufAnfrageAnUenb = "11192";
        public const string UTILMD_11193_AblehnungAnfrageAnUenb = "11193";
        public const string UTILMD_11194_AntwortAufDieGeschaeftsdatenanfrageAnDenMsbGas = "11194";
        public const string UTILMD_11195_Mabis_Bilanzierungsgebietsclearingliste = "11195";
        public const string UTILMD_11196_Mabis_AntwortAufDieBilanzierungsgebietsclearingliste =
            "11196";
        public const string UTILMD_11197_Redispatch_AktivierungZpTaeglicheAauez = "11197";
        public const string UTILMD_11198_Redispatch_DeaktivierungTaeglicheAauez = "11198";
        public const string UTILMD_11199_Redispatch_AktivierungZpLfAaszr = "11199";
        public const string UTILMD_11200_Redispatch_DeaktivierungZpLfAaszr = "11200";
        public const string UTILMD_11201_Redispatch_LfAacl = "11201";
        public const string UTILMD_11202_Redispatch_KorrekturlisteLfAacl = "11202";
        public const string UTILMD_11203_Redispatch_AktivierungZpMonatlicheAauez = "11203";
        public const string UTILMD_11204_Redispatch_AntwortAufAktivierungZp = "11204";
        public const string UTILMD_11205_Redispatch_WeiterleitungAktivierungZp = "11205";
        public const string UTILMD_11206_Redispatch_DeaktivierungZpMonatlicheAauez = "11206";
        public const string UTILMD_11207_Redispatch_AntwortAufDeaktivierungZp = "11207";
        public const string UTILMD_11208_Redispatch_WeiterleitungDeaktivierungZp = "11208";
        public const string UTILMD_11209_Redispatch_AktivierungZpMonatlicheAauez = "11209";
        public const string UTILMD_11210_Redispatch_AntwortAufAktivierungZp = "11210";
        public const string UTILMD_11211_Redispatch_WeiterleitungAktivierungZp = "11211";
        public const string UTILMD_11212_Redispatch_DeaktivierungZpMonatlicheAauez = "11212";
        public const string UTILMD_11213_Redispatch_AntwortAufDeaktivierungZp = "11213";
        public const string UTILMD_11214_Redispatch_WeiterleitungDeaktivierungZp = "11214";
        public const string UTILMD_11215_Gpke_AntwortAufDieGeschaeftsdatenanfrageDesUenb = "11215";
        public const string UTILMD_11216_AntwortBestellungZaehlzeitdefinitionDesLf = "11216";
        public const string UTILMD_11217_AntwortBestellungZaehlzeitdefinitionDesLf = "11217";
        public const string UTILMD_11218_Sdae_NichtBilarelAenderungNnaVomNbAnMarktlokation =
            "11218";
        public const string UTILMD_11219_Sdae_AntwortAufAenderungNnaVomNbAnMarktlokation = "11219";
        public const string UTILMD_11220_Sdae_NichtBilarelAnfrageNnaAnNbAnMarktlokation = "11220";
        public const string UTILMD_11221_Sdae_AntwortAufAnfrageNnaAnNbAnMarktlokation = "11221";
        public const string UTILMD_11222_Sdae_AblehnungAnfrageNnaAnNbAnMarktlokation = "11222";
        public const string UTILMD_11223_Mabis_DzueListe = "11223";
        public const string UTILMD_11224_Mabis_AntwortAufDzueListe = "11224";
        public const string UTILMD_11225_Sdae_NichtBilarelAenderungAbrechnungBlindarbeitVomNb =
            "11225";
        public const string UTILMD_11226_Sdae_AntwortAufAenderungAbrechnungBlindarbeitVomNb =
            "11226";
        public const string UTILMD_11227_Sdae_NichtBilarelAnfrageAbrechnungBlindarbeitAnNb =
            "11227";
        public const string UTILMD_11228_Sdae_AntwortAufAnfrageAbrechnungBlindarbeitAnNb = "11228";
        public const string UTILMD_11229_Sdae_AblehnungAnfrageAbrechnungBlindarbeitAnNb = "11229";
        public const string UTILMD_11230_Sdae_NichtBilarelAenderungAbrechnungBlindarbeitVomLf =
            "11230";
        public const string UTILMD_11231_Sdae_AntwortAufAenderungAbrechnungBlindarbeitVomLf =
            "11231";
        public const string UTILMD_11232_Sdae_NichtBilarelAnfrageAbrechnungBlindarbeitAnLf =
            "11232";
        public const string UTILMD_11233_Sdae_AntwortAufAnfrageAbrechnungBlindarbeitAnLf = "11233";
        public const string UTILMD_11234_Sdae_AblehnungAnfrageAbrechnungBlindarbeitAnLf = "11234";

        #endregion

        #region UTILMDS

        public const string UTILMD_55065_Mabis_Lieferantenclearingliste = "55065";
        public const string UTILMD_55066_Mabis_KorrekturlisteZuLieferantenclearingliste = "55066";
        public const string UTILMD_55067_Mabis_Bilanzkreiszuordnungsliste = "55067";
        public const string UTILMD_55069_Mabis_ClearinglisteDzr = "55069";
        public const string UTILMD_55070_Mabis_ClearinglisteBas = "55070";
        public const string UTILMD_55073_Mabis_UebermittlungDerProfildefinitionen = "55073";
        public const string UTILMD_55195_Mabis_Bilanzierungsgebietsclearingliste = "55195";
        public const string UTILMD_55196_Mabis_AntwortAufDieBilanzierungsgebietsclearingliste =
            "55196";
        public const string UTILMD_55201_Redispatch_LfAacl = "55201";
        public const string UTILMD_55202_Redispatch_KorrekturlisteLfAacl = "55202";
        public const string UTILMD_55223_Mabis_DzueListe = "55223";
        public const string UTILMD_55224_Mabis_AntwortAufDzueListe = "55224";
        public const string UTILMD_55001_Gpke_AnmeldungNn = "55001";
        public const string UTILMD_55002_Gpke_BestaetigungAnmeldung = "55002";
        public const string UTILMD_55003_Gpke_AblehnungAnmeldung = "55003";
        public const string UTILMD_55004_Gpke_AbmeldungNn = "55004";
        public const string UTILMD_55005_Gpke_BestaetigungAbmeldung = "55005";
        public const string UTILMD_55006_Gpke_AblehnungAbmeldung = "55006";
        public const string UTILMD_55007_Gpke_AbmeldungNnVomNb = "55007";
        public const string UTILMD_55008_Gpke_BestaetigungAbmeldungVomNb = "55008";
        public const string UTILMD_55009_Gpke_AblehnungAbmeldungVomNb = "55009";
        public const string UTILMD_55010_Gpke_AbmeldeanfrageDesNb = "55010";
        public const string UTILMD_55011_Gpke_BestaetigungAbmeldeanfrage = "55011";
        public const string UTILMD_55012_Gpke_AblehnungAbmeldeanfrage = "55012";
        public const string UTILMD_55013_Gpke_AnmeldungEog = "55013";
        public const string UTILMD_55014_Gpke_BestaetigungEogAnmeldung = "55014";
        public const string UTILMD_55015_Gpke_AblehnungEogAnmeldung = "55015";
        public const string UTILMD_55016_Gpke_KuendigungBeimAltenLieferanten = "55016";
        public const string UTILMD_55017_Gpke_BestaetigungKuendigung = "55017";
        public const string UTILMD_55018_Gpke_AblehnungKuendigung = "55018";
        public const string UTILMD_55022_Gpke_AnfrageNachStornierung = "55022";
        public const string UTILMD_55023_Gpke_BestaetigungAnfrageStornierung = "55023";
        public const string UTILMD_55024_Gpke_AblehnungAnfrageStornierung = "55024";
        public const string UTILMD_55035_Gpke_AntwortAufDieGeschaeftsdatenanfrage = "55035";
        public const string UTILMD_55036_Gpke_InformationsmeldungUeberExistierendeZuordnung =
            "55036";
        public const string UTILMD_55037_Gpke_InformationsmeldungZurBeendigungDerZuordnung =
            "55037";
        public const string UTILMD_55038_Gpke_InformationsmeldungZurAufhebungEinerZukZuordnung =
            "55038";
        public const string UTILMD_55039_WimStrom_KuendigungMsb = "55039";
        public const string UTILMD_55040_WimStrom_BestaetigungKuendigungMsb = "55040";
        public const string UTILMD_55041_WimStrom_AblehnungKuendigungMsb = "55041";
        public const string UTILMD_55042_WimStrom_AnmeldungMsb = "55042";
        public const string UTILMD_55043_WimStrom_BestaetigungAnmeldungMsb = "55043";
        public const string UTILMD_55044_WimStrom_AblehnungAnmeldungMsb = "55044";
        public const string UTILMD_55051_WimStrom_EndeMsb = "55051";
        public const string UTILMD_55052_WimStrom_BestaetigungEndeMsb = "55052";
        public const string UTILMD_55053_WimStrom_AblehnungEndeMsb = "55053";
        public const string UTILMD_55060_WimStrom_AntwortAufDieMsbGeschaeftsdatenanfrage = "55060";
        public const string UTILMD_55062_Mabis_AktivierungVonMabisZp = "55062";
        public const string UTILMD_55063_Mabis_DeaktivierungVonMabisZp = "55063";
        public const string UTILMD_55064_Mabis_AntwortAufAktivierung_DeaktivierungVonMabisZp =
            "55064";
        public const string UTILMD_55071_Mabis_AktivierungDerZuordnungsermaechtigung = "55071";
        public const string UTILMD_55072_Mabis_DeaktivierungDerZuordnungsermaechtigung = "55072";
        public const string UTILMD_55074_Hknr_StammdatenAufEineOrders = "55074";
        public const string UTILMD_55075_Hknr_StammdatenAufgrundEinerAenderung = "55075";
        public const string UTILMD_55076_Hknr_AntwortAufStammdatenaenderung = "55076";
        public const string UTILMD_55077_Mpes_Anmeldung = "55077";
        public const string UTILMD_55078_Mpes_BestaetigungAnmeldung = "55078";
        public const string UTILMD_55079_Mpes_BestaetigungAnmeldungNeuanlageUndBestaetigungLf =
            "55079";
        public const string UTILMD_55080_Mpes_AblehnungAnmeldung = "55080";
        public const string UTILMD_55081_Mpes_Abmeldung = "55081";
        public const string UTILMD_55082_Mpes_BestaetigungAbmeldung = "55082";
        public const string UTILMD_55083_Mpes_AblehnungAbmeldung = "55083";
        public const string UTILMD_55084_Mpes_AbmeldungStilllegung = "55084";
        public const string UTILMD_55085_Mpes_AntwortAbmeldungStilllegung = "55085";
        public const string UTILMD_55086_Mpes_AbmeldeanfrageDesNb = "55086";
        public const string UTILMD_55087_Mpes_BestaetigungAbmeldeanfrage = "55087";
        public const string UTILMD_55088_Mpes_AblehnungAbmeldeanfrage = "55088";
        public const string UTILMD_55089_Mpes_KuendigungBeimAltenLieferanten = "55089";
        public const string UTILMD_55090_Mpes_BestaetigungKuendigung = "55090";
        public const string UTILMD_55091_Mpes_AblehnungKuendigung = "55091";
        public const string UTILMD_55092_Mpes_InformationsmeldungUeberExistierendeZuordnung =
            "55092";
        public const string UTILMD_55093_Mpes_InformationsmeldungZurBeendigungDerZuordnung =
            "55093";
        public const string UTILMD_55094_Mpes_InformationsmeldungZurAufhebungEinerZukZuordnung =
            "55094";
        public const string UTILMD_55095_Mpes_AntwortAufDieGeschaeftsdatenanfrage = "55095";
        public const string UTILMD_55101_NbwStrom_StammdatenZurMesslokation = "55101";
        public const string UTILMD_55102_NbwStrom_AktualisierteStammdatenZurMesslokation = "55102";
        public const string UTILMD_55103_NbwStrom_StammdatenZurVerbrauchendenMarktlokation =
            "55103";
        public const string UTILMD_55104_NbwStrom_AktualisierteStammdatenZurVerbrauchendenMarktlokation =
            "55104";
        public const string UTILMD_55105_NbwStrom_AblehnungAufStammdatenZurVerbrauchendenMarktlokation =
            "55105";
        public const string UTILMD_55106_NbwStrom_StammdatenZurErzeugendenMarktlokation = "55106";
        public const string UTILMD_55107_NbwStrom_AktualisierteStammdatenZurErzeugendenMarktlokation =
            "55107";
        public const string UTILMD_55108_NbwStrom_AblehnungAufStammdatenZurErzeugendenMarktlokation =
            "55108";
        public const string UTILMD_55109_SdaeStrom_NichtBilarelAenderungVomLfAnNb = "55109";
        public const string UTILMD_55110_SdaeStrom_NichtBilarelAenderungVomLfNbAnMsb = "55110";
        public const string UTILMD_55111_SdaeStrom_AntwortAufAenderungVomLfAg = "55111";
        public const string UTILMD_55112_SdaeStrom_NichtBilarelAenderungVomNbAnLf = "55112";
        public const string UTILMD_55113_SdaeStrom_NichtBilarelAenderungVomNbAnMsb = "55113";
        public const string UTILMD_55115_SdaeStrom_AntwortAufAenderungVomNbAgAnNb = "55115";
        public const string UTILMD_55116_SdaeStrom_AenderungVomMsbAnNb = "55116";
        public const string UTILMD_55117_SdaeStrom_AenderungVomMsbNbAnLf = "55117";
        public const string UTILMD_55119_SdaeStrom_AntwortAufAenderungVomMsbAgAnAf = "55119";
        public const string UTILMD_55120_SdaeStrom_BilarelAenderungVomLfAnNb = "55120";
        public const string UTILMD_55121_SdaeStrom_AntwortAufBilarelAenderungVomLfNbAnLf = "55121";
        public const string UTILMD_55123_SdaeStrom_BilarelAenderungVomNbMitAbhaengigkeitenNbAnLf_Msb =
            "55123";
        public const string UTILMD_55124_SdaeStrom_AntwortAufBilarelAenderungVomNbMitAbhaengigkeitenAgAnNb =
            "55124";
        public const string UTILMD_55126_SdaeStrom_AenderungDerPrognosegrundlageNbAnLf_Msb =
            "55126";
        public const string UTILMD_55127_SdaeStrom_AntwortAufAenderungDerPrognosegrundlageAgAnNb =
            "55127";
        public const string UTILMD_55133_SdaeStrom_NichtBilarelAnfrageAnLfMsbAnNb = "55133";
        public const string UTILMD_55134_SdaeStrom_AntwortAufNichtBilarelAnfrageAnLfNbAnMsb =
            "55134";
        public const string UTILMD_55135_SdaeStrom_AntwortAufNichtBilarelAnfrageAnLfNbAnMsb =
            "55135";
        public const string UTILMD_55136_SdaeStrom_NichtBilarelAnfrageAnLfNbVerteilerAnLf = "55136";
        public const string UTILMD_55137_SdaeStrom_NichtBilarelAnfrageAnLfNbBerechtigtAnLf =
            "55137";
        public const string UTILMD_55138_SdaeStrom_AntwortAufNichtBilarelAnfrageAnLfLfAnNb =
            "55138";
        public const string UTILMD_55139_SdaeStrom_NichtBilarelAnfrageAnNbLfAnNb = "55139";
        public const string UTILMD_55140_SdaeStrom_NichtBilarelAnfrageAnNbMsbAnNb = "55140";
        public const string UTILMD_55142_SdaeStrom_AntwortAufNichtBilarelAnfrageAnNbNbAnAf =
            "55142";
        public const string UTILMD_55143_SdaeStrom_AnfrageAnMsbMitAbhaengigkeitenLfAnNb = "55143";
        public const string UTILMD_55145_SdaeStrom_AntwortAufAnfrageAnMsbMitAbhaengigkeitenNbAnAf =
            "55145";
        public const string UTILMD_55146_SdaeStrom_AblehnungDerAnfrageAnMsbMitAbhaengigkeitenNbAnAf =
            "55146";
        public const string UTILMD_55147_SdaeStrom_AnfrageAnMsbMitAbhaengigkeitenNbVerteilerAnMsb =
            "55147";
        public const string UTILMD_55148_SdaeStrom_AnfrageAnMsbMitAbhaengigkeitenNbBerechtigtAnMsb =
            "55148";
        public const string UTILMD_55149_SdaeStrom_AntwortAufAnfrageAnMsbMitAbhaengigkeitenMsbAnNb =
            "55149";
        public const string UTILMD_55150_SdaeStrom_BilarelAnfrageAnLfNbAnLf = "55150";
        public const string UTILMD_55151_SdaeStrom_AntwortAufBilarelAnfrageAnLfLfAnNb = "55151";
        public const string UTILMD_55152_SdaeStrom_AblehnungDerBilarelAnfrage = "55152";
        public const string UTILMD_55153_SdaeStrom_BilarelAnfrageAnNbOhneAbhaengigkeiten = "55153";
        public const string UTILMD_55154_SdaeStrom_AntwortAufBilarelAnfrageAnNbOhneAbhaengigkeiten =
            "55154";
        public const string UTILMD_55155_SdaeStrom_AblehnungDerBilarelAnfrageAnNbOhneAbhaengigkeiten =
            "55155";
        public const string UTILMD_55156_SdaeStrom_BilarelAnfrageAnNbMitAbhaengigkeiten = "55156";
        public const string UTILMD_55157_SdaeStrom_AntwortAufBilarelAnfrageAnNbMitAbhaengigkeiten =
            "55157";
        public const string UTILMD_55159_SdaeStrom_AenderungVomMsbOhneAbhaengigkeitenMsbAnNb =
            "55159";
        public const string UTILMD_55160_SdaeStrom_AenderungVomMsbOhneAbhaengigkeitenNbAnLf_Msb =
            "55160";
        public const string UTILMD_55161_SdaeStrom_AenderungVomMsbOhneAbhaengigkeitenAgAnAf =
            "55161";
        public const string UTILMD_55162_SdaeStrom_AnfrageAnMsbOhneAbhaengigkeitenVonLf_MsbAnNb =
            "55162";
        public const string UTILMD_55163_SdaeStrom_AntwortAufAnfrageAnMsbOhneAbhaengigkeitenLf_MsbAnNb =
            "55163";
        public const string UTILMD_55164_SdaeStrom_AblehnungAufAnfrageAnMsbOhneAbhaengigkeitenLf_MsbAnNb =
            "55164";
        public const string UTILMD_55165_SdaeStrom_AnfrageAnMsbOhneAbhaengigkeitenNbVerteilerAnMsb =
            "55165";
        public const string UTILMD_55166_SdaeStrom_AnfrageAnMsbOhneAbhaengigkeitenNbBerechtigterAnMsb =
            "55166";
        public const string UTILMD_55167_SdaeStrom_AntwortAufAnfrageAnMsbOhneAbhaengigkeitenMsbAnNb =
            "55167";
        public const string UTILMD_55168_WimStrom_Verpflichtungsanfrage_Aufforderung = "55168";
        public const string UTILMD_55169_WimStrom_BestaetigungVerpflichtungsanfrage = "55169";
        public const string UTILMD_55170_WimStrom_AblehnungVerpflichtungsanfrage = "55170";
        public const string UTILMD_55171_SdaeStrom_AenderungVomMsbNbAnMsbBerechtigter = "55171";
        public const string UTILMD_55172_SdaeStrom_AnfrageAnMsbMitAbhaengigkeitMsbAnNb = "55172";
        public const string UTILMD_55173_SdaeStrom_AenderungDerLokationsbuendelstrukturNbAnMsb =
            "55173";
        public const string UTILMD_55174_SdaeStrom_AntwortAufAenderungDerLokationsbuendelstrukturMsbAnNb =
            "55174";
        public const string UTILMD_55175_SdaeStrom_AenderungDerMarktlokationsstrukturNbAnLf =
            "55175";
        public const string UTILMD_55176_SdaeStrom_AntwortAufAenderungDerMarktlokationsstrukturLfAnNb =
            "55176";
        public const string UTILMD_55177_SdaeStrom_AnfrageDerLokationsbuendelstrukturMsbAnNb =
            "55177";
        public const string UTILMD_55178_SdaeStrom_AntwortAufAnfrageDerLokationsbuendelstrukturNbAnMsb =
            "55178";
        public const string UTILMD_55180_SdaeStrom_AnfrageDerKomplexenMarktlokationsstrukturLfAnNb =
            "55180";
        public const string UTILMD_55181_SdaeStrom_AntwortAufAnfrageDerKomplexenMarktlokationsstrukturNbAnLf =
            "55181";
        public const string UTILMD_55182_SdaeStrom_AblehnungDerAnfrageDer = "55182";
        public const string UTILMD_55185_SdaeStrom_StammdatensynchronisationVomNb = "55185";
        public const string UTILMD_55186_SdaeStrom_StammdatensynchronisationVomLf = "55186";
        public const string UTILMD_55187_SdaeStrom_StammdatensynchronisationVomUenb = "55187";
        public const string UTILMD_55188_SdaeStrom_BeendigungDerAggregationsverantwortungVomNb =
            "55188";
        public const string UTILMD_55189_SdaeStrom_BeendigungDerAggregationsverantwortungVomLf =
            "55189";
        public const string UTILMD_55190_SdaeStrom_BeendigungDerAggregationsverantwortungVomUenb =
            "55190";
        public const string UTILMD_55191_SdaeStrom_AnfrageAnNbVomUenb = "55191";
        public const string UTILMD_55192_SdaeStrom_AntwortAufAnfrageAnUenb = "55192";
        public const string UTILMD_55193_SdaeStrom_AblehnungAnfrageAnUenb = "55193";
        public const string UTILMD_55194_Gpke_AntwortAufDieGeschaeftsdatenanfrageAnDenMsbGas =
            "55194";
        public const string UTILMD_55197_Redispatch_AktivierungZpTaeglicheAauez = "55197";
        public const string UTILMD_55198_Redispatch_DeaktivierungTaeglicheAauez = "55198";
        public const string UTILMD_55199_Redispatch_AktivierungZpLfAaszr = "55199";
        public const string UTILMD_55200_Redispatch_DeaktivierungZpLfAaszr = "55200";
        public const string UTILMD_55203_Redispatch_AktivierungZpMonatlicheAauez = "55203";
        public const string UTILMD_55204_Redispatch_AntwortAufAktivierungZp = "55204";
        public const string UTILMD_55205_Redispatch_WeiterleitungAktivierungZp = "55205";
        public const string UTILMD_55206_Redispatch_DeaktivierungZpMonatlicheAauez = "55206";
        public const string UTILMD_55207_Redispatch_AntwortAufDeaktivierungZp = "55207";
        public const string UTILMD_55208_Redispatch_WeiterleitungDeaktivierungZp = "55208";
        public const string UTILMD_55209_Redispatch_AktivierungZpMonatlicheAauez = "55209";
        public const string UTILMD_55210_Redispatch_AntwortAufAktivierungZp = "55210";
        public const string UTILMD_55211_Redispatch_WeiterleitungAktivierungZp = "55211";
        public const string UTILMD_55212_Redispatch_DeaktivierungZpMonatlicheAauez = "55212";
        public const string UTILMD_55213_Redispatch_AntwortAufDeaktivierungZp = "55213";
        public const string UTILMD_55214_Redispatch_WeiterleitungDeaktivierungZp = "55214";
        public const string UTILMD_55215_Gpke_AntwortAufDieGeschaeftsdatenanfrageDesUenb = "55215";
        public const string UTILMD_55218_SdaeStrom_NichtBilarelAenderungNnaVomNbAnMarktlokation =
            "55218";
        public const string UTILMD_55219_SdaeStrom_AntwortAufAenderungNnaVomNbAnMarktlokation =
            "55219";
        public const string UTILMD_55220_SdaeStrom_NichtBilarelAnfrageNnaAnNbAnMarktlokation =
            "55220";
        public const string UTILMD_55221_SdaeStrom_AntwortAufAnfrageNnaAnNbAnMarktlokation =
            "55221";
        public const string UTILMD_55222_SdaeStrom_AblehnungAnfrageNnaAnNbAnMarktlokation = "55222";
        public const string UTILMD_55225_SdaeStrom_NichtBilarelAenderungAbrechnungBlindarbeitVomNb =
            "55225";
        public const string UTILMD_55226_SdaeStrom_AntwortAufAenderungAbrechnungBlindarbeitVomNb =
            "55226";
        public const string UTILMD_55227_SdaeStrom_NichtBilarelAnfrageAbrechnungBlindarbeitAnNb =
            "55227";
        public const string UTILMD_55228_SdaeStrom_AntwortAufAnfrageAbrechnungBlindarbeitAnNb =
            "55228";
        public const string UTILMD_55229_SdaeStrom_AblehnungAnfrageAbrechnungBlindarbeitAnNb =
            "55229";
        public const string UTILMD_55230_SdaeStrom_NichtBilarelAenderungAbrechnungBlindarbeitVomLf =
            "55230";
        public const string UTILMD_55231_SdaeStrom_AntwortAufAenderungAbrechnungBlindarbeitVomLf =
            "55231";
        public const string UTILMD_55232_SdaeStrom_NichtBilarelAnfrageAbrechnungBlindarbeitAnLf =
            "55232";
        public const string UTILMD_55233_SdaeStrom_AntwortAufAnfrageAbrechnungBlindarbeitAnLf =
            "55233";
        public const string UTILMD_55234_SdaeStrom_AblehnungAnfrageAbrechnungBlindarbeitAnLf =
            "55234";
        public const string UTILMD_55557_SdaeStrom_AenderungVonMSBAbrechnungsdaten = "55557";
        public const string UTILMD_55558_SdaeStrom_AntwortAufAenderungVonMSBAbrechnungsdaten =
            "55558";
        public const string UTILMD_55559_SdaeStrom_AnfrageVonMSBAbrechnungsdaten = "55559";
        public const string UTILMD_55560_SdaeStrom_AntwortAufAnfrageVonMSBAbrechnungsdaten =
            "55560";
        public const string UTILMD_55561_SdaeStrom_AblehnungDerAnfrageVonMSBAbrechnungsdaten =
            "55561";
        public const string UTILMD_55235_Mabis_ZuordnungZpDerNgzZurNzr = "55235";
        public const string UTILMD_55236_Mabis_BeendigungZuordnungZpDerNgzZurNzr = "55236";
        public const string UTILMD_55237_Mabis_AntwortAufZuordnung_BeendigungZuordnungZpDerNgzZurNzr =
            "55237";
        public const string UTILMD_55238_E_Mob_AnmeldungInModell2 = "55238";
        public const string UTILMD_55239_E_Mob_AntwortAufAnmeldungInModell2 = "55239";
        public const string UTILMD_55240_E_Mob_BeendigungDerZuordnungZurMarktlokation = "55240";
        public const string UTILMD_55241_E_Mob_AntwortAufBeendigungDerZuordnungZurMarktlokation =
            "55241";
        public const string UTILMD_55242_E_Mob_AbmeldungAusDemModell2 = "55242";
        public const string UTILMD_55243_E_Mob_AntwortAufAbmeldungAusDemModell2 = "55243";
        public const string UTILMD_55553_Gpke_StammdatenAufIndividuelleBestellung = "55553";
        public const string UTILMD_55554_Gpke_AntwortAufAenderungDerIndividuellenBestellung =
            "55554";
        public const string UTILMD_55555_Gpke_AnfrageStammdatenAufIndividuelleBestellung = "55555";
        public const string UTILMD_55556_Gpke_AntwortAufAnfrageDerIndividuellenBestellung = "55556";

        public const string UTILMD_55600_Gpke_AnmeldungNeuerVerbMaloLfAnNb = "55600";
        public const string UTILMD_55601_Gpke_AnmeldungNeuerErzMaloLfAnNb = "55601";
        public const string UTILMD_55602_Gpke_BestaetigungAnmeldungNeuerVerbMaloNbAnLf = "55602";
        public const string UTILMD_55603_Gpke_BestaetigungAnmeldungNeuerErzMaloNbAnLf = "55603";
        public const string UTILMD_55604_Gpke_AblehnungAnmeldungNeuerVerbMaloNbAnLf = "55604";
        public const string UTILMD_55605_Gpke_AblehnungAnmeldungNeuerErzMaloNbAnLf = "55605";
        public const string UTILMD_55607_Gpke_AnkuendigungZuordnung_ZuordnungDesLfZurMalo_TrancheNbAnLfn =
            "55607";
        public const string UTILMD_55608_Gpke_BestaetigungZuordnungDesLfZurMalo_TrancheLfnAnNb =
            "55608";
        public const string UTILMD_55609_Gpke_AblehnungZuordnungDesLfZurMalo_TrancheLfnAnNb =
            "55609";
        public const string UTILMD_55611_Gpke_BeendigungDerZuordnungNbAnMsb = "55611";
        public const string UTILMD_55613_Gpke_AbrDatenBkAbrVerbMaloNbAnÜnb = "55613";
        public const string UTILMD_55614_Gpke_Rueckmeldung_AnfrageAbrDatenBkAbrVerbMaloUenbAnNb =
            "55614";
        public const string UTILMD_55615_Gpke_AenderungDatenDerNeloNbAnLf = "55615";
        public const string UTILMD_55616_Gpke_AenderungDatenDerMaloNbAnLf = "55616";
        public const string UTILMD_55617_Gpke_AenderungDatenDerTrNbAnLf = "55617";
        public const string UTILMD_55618_Gpke_AenderungDatenDerSrNbAnLf = "55618";
        public const string UTILMD_55619_Gpke_AenderungDatenDerTrancheNbAnLf = "55619";
        public const string UTILMD_55620_Gpke_AenderungDatenDerMeloNbAnLf = "55620";
        public const string UTILMD_55621_Gpke_Rueckmeldung_AnfrageDatenZurNeloLfAnNb = "55621";
        public const string UTILMD_55622_Gpke_Rueckmeldung_AnfrageDatenDerMaloLfAnNb = "55622";
        public const string UTILMD_55623_Gpke_Rueckmeldung_AnfrageDatenDerTrLfAnNb = "55623";
        public const string UTILMD_55624_Gpke_Rueckmeldung_AnfrageDatenDerSrLfAnNb = "55624";
        public const string UTILMD_55625_Gpke_Rueckmeldung_AnfrageDatenDerTrancheLfAnNb = "55625";
        public const string UTILMD_55626_Gpke_Rueckmeldung_AnfrageDatenDerMeloLfAnNb = "55626";
        public const string UTILMD_55627_Gpke_AenderungDatenDerNeloNbAnMsb = "55627";
        public const string UTILMD_55628_Gpke_AenderungDatenDerMaloNbAnMsb = "55628";
        public const string UTILMD_55629_Gpke_AenderungDatenDerTrNbAnMsb = "55629";
        public const string UTILMD_55630_Gpke_AenderungDatenDerSrNbAnMsb = "55630";
        public const string UTILMD_55632_Gpke_AenderungDatenDerMeloNbAnMsb = "55632";
        public const string UTILMD_55633_Gpke_Rueckmeldung_AnfrageDatenZurNeloMsbAnNb = "55633";
        public const string UTILMD_55634_Gpke_Rueckmeldung_AnfrageDatenDerMaloMsbAnNb = "55634";
        public const string UTILMD_55635_Gpke_Rueckmeldung_AnfrageDatenDerTrMsbAnNb = "55635";
        public const string UTILMD_55636_Gpke_Rueckmeldung_AnfrageDatenDerSrMsbAnNb = "55636";
        public const string UTILMD_55638_Gpke_Rueckmeldung_AnfrageDatenDerMeloMsbAnNb = "55638";
        public const string UTILMD_55639_Gpke_AenderungDatenDerNeloMsbAnNb = "55639";
        public const string UTILMD_55640_Gpke_AenderungDatenDerMaloMsbAnNb = "55640";
        public const string UTILMD_55641_Gpke_AenderungDatenDerSrMsbAnNb = "55641";
        public const string UTILMD_55642_Gpke_AenderungDatenDerTrancheMsbAnNb = "55642";
        public const string UTILMD_55643_Gpke_AenderungDatenDerMeloMsbAnNb = "55643";
        public const string UTILMD_55644_Gpke_Rueckmeldung_AnfrageDatenDerNeloNbAnMsb = "55644";
        public const string UTILMD_55645_Gpke_Rueckmeldung_AnfrageDatenDerMaloNbAnMsb = "55645";
        public const string UTILMD_55646_Gpke_Rueckmeldung_AnfrageDatenDerSrNbAnMsb = "55646";
        public const string UTILMD_55647_Gpke_Rueckmeldung_AnfrageDatenDerTrancheNbAnMsb = "55647";
        public const string UTILMD_55648_Gpke_Rueckmeldung_AnfrageDatenDerMeloNbAnMsb = "55648";
        public const string UTILMD_55649_Gpke_AenderungDatenDerNeloMsbAnLf = "55649";
        public const string UTILMD_55650_Gpke_AenderungDatenDerMaloMsbAnLf = "55650";
        public const string UTILMD_55651_Gpke_AenderungDatenDerSrMsbAnLf = "55651";
        public const string UTILMD_55652_Gpke_AenderungDatenDerTrancheMsbAnLf = "55652";
        public const string UTILMD_55653_Gpke_AenderungDatenDerMeloMsbAnLf = "55653";
        public const string UTILMD_55654_Gpke_Rueckmeldung_AnfrageDatenDerNeloLfAnMsb = "55654";
        public const string UTILMD_55655_Gpke_Rueckmeldung_AnfrageDatenDerMaloLfAnMsb = "55655";
        public const string UTILMD_55656_Gpke_Rueckmeldung_AnfrageDatenDerSrLfAnMsb = "55656";
        public const string UTILMD_55657_Gpke_Rueckmeldung_AnfrageDatenDerTrancheLfAnMsb = "55657";
        public const string UTILMD_55658_Gpke_Rueckmeldung_AnfrageDatenDerMeloLfAnMsb = "55658";
        public const string UTILMD_55659_Gpke_AenderungDatenDerNeloMsbAnWeitererMsb = "55659";
        public const string UTILMD_55660_Gpke_AenderungDatenDerMaloMsbAnWeitererMsb = "55660";
        public const string UTILMD_55661_Gpke_AenderungDatenDerSrMsbAnWeitererMsb = "55661";
        public const string UTILMD_55662_Gpke_AenderungDatenDerTrancheMsbAnWeitererMsb = "55662";
        public const string UTILMD_55663_Gpke_AenderungDatenDerMeloMsbAnWeitererMsb = "55663";
        public const string UTILMD_55664_Gpke_Rueckmeldung_AnfrageDatenDerNeloWeitererMsbAnMsb =
            "55664";
        public const string UTILMD_55665_Gpke_Rueckmeldung_AnfrageDatenDerMaloWeitererMsbAnMsb =
            "55665";
        public const string UTILMD_55666_Gpke_Rueckmeldung_AnfrageDatenDerSrWeitererMsbAnMsb =
            "55666";
        public const string UTILMD_55667_Gpke_Rueckmeldung_AnfrageDatenDerTrancheWeitererMsbAnMsb =
            "55667";
        public const string UTILMD_55669_Gpke_Rueckmeldung_AnfrageDatenDerMeloWeitererMsbAnMsb =
            "55669";
        public const string UTILMD_55670_Gpke_StammdatenBkTreueNbAnUenb = "55670";
        public const string UTILMD_55671_Gpke_RueckmeldungAufStammdatenBkTreueUenbAnNb = "55671";
        public const string UTILMD_55672_Gpke_AbrDatenBkAbrErzMaloNbAnLf = "55672";
        public const string UTILMD_55673_Gpke_Rueckmeldung_AnfrageAbrDatenBkAbrErzMaloLfAnNb =
            "55673";
        public const string UTILMD_55674_Gpke_AbrDatenBkAbrErzMaloNbAnUenb = "55674";
        public const string UTILMD_55675_Gpke_Rueckmeldung_AnfrageAbrDatenBkAbrErzMaloUenbAnNb =
            "55675";
        public const string UTILMD_55684_Gpke_AenderungDatenDerMaloMsbAnUenb = "55684";
        public const string UTILMD_55685_Gpke_Rueckmeldung_AnfrageDatenDerMaloUenbAnMsb = "55685";
        public const string UTILMD_55686_Gpke_AenderungDatenDerTrancheMsbAnUenb = "55686";
        public const string UTILMD_55687_Gpke_Rueckmeldung_AnfrageDatenDerTrancheUenbAnMsb =
            "55687";
        public const string UTILMD_55688_Gpke_AenderungDatenDerMaloNbAnUenb = "55688";
        public const string UTILMD_55689_Gpke_Rueckmeldung_AnfrageDatenDerMaloUenbAnNb = "55689";
        public const string UTILMD_55690_AwhNetzbetreiberwechsel_LokationsbuendelstrukturUndDbNbaAnNbn =
            "55690";
        public const string UTILMD_55691_GpkeTeil4_AenderungPaketIdDerMaloNbAnLf = "55691";
        public const string UTILMD_55692_GpkeTeil4_RueckmeldungAnfragePaketIdDerMaloLfAnNb =
            "55692";

        #endregion

        #region UTILMDG


        public const string UTILMD_44001_GeliGas_AnmeldungNn = "44001";
        public const string UTILMD_44002_GeliGas_BestaetigungAnmeldung = "44002";
        public const string UTILMD_44003_GeliGas_AblehnungAnmeldung = "44003";
        public const string UTILMD_44004_GeliGas_AbmeldungNn = "44004";
        public const string UTILMD_44005_GeliGas_BestaetigungAbmeldung = "44005";
        public const string UTILMD_44006_GeliGas_AblehnungAbmeldung = "44006";
        public const string UTILMD_44007_GeliGas_AbmeldungNnVomNb = "44007";
        public const string UTILMD_44008_GeliGas_BestaetigungAbmeldungVomNb = "44008";
        public const string UTILMD_44009_GeliGas_AblehnungAbmeldungVomNb = "44009";
        public const string UTILMD_44010_GeliGas_AbmeldeanfrageDesNb = "44010";
        public const string UTILMD_44011_GeliGas_BestaetigungAbmeldeanfrage = "44011";
        public const string UTILMD_44012_GeliGas_AblehnungAbmeldeanfrage = "44012";
        public const string UTILMD_44013_GeliGas_AnmeldungEog = "44013";
        public const string UTILMD_44014_GeliGas_BestaetigungEogAnmeldung = "44014";
        public const string UTILMD_44015_GeliGas_AblehnungEogAnmeldung = "44015";
        public const string UTILMD_44016_GeliGas_KuendigungBeimAltenLieferanten = "44016";
        public const string UTILMD_44017_GeliGas_BestaetigungKuendigung = "44017";
        public const string UTILMD_44018_GeliGas_AblehnungKuendigung = "44018";
        public const string UTILMD_44019_GeliGas_BestandslisteZugeordneteMarktlokationen = "44019";
        public const string UTILMD_44020_GeliGas_AenderungsmeldungZurBestandsliste = "44020";
        public const string UTILMD_44021_GeliGas_AntwortAufAenderungsmeldungZurBestandsliste =
            "44021";
        public const string UTILMD_44022_GeliGas_AnfrageNachStornierung = "44022";
        public const string UTILMD_44023_GeliGas_BestaetigungAnfrageStornierung = "44023";
        public const string UTILMD_44024_GeliGas_AblehnungAnfrageStornierung = "44024";
        public const string UTILMD_44035_GeliGas_AntwortAufDieGeschaeftsdatenanfrage = "44035";
        public const string UTILMD_44036_GeliGas_InformationsmeldungUeberExistierendeZuordnung =
            "44036";
        public const string UTILMD_44037_GeliGas_InformationsmeldungZurBeendigungDerZuordnung =
            "44037";
        public const string UTILMD_44038_GeliGas_InformationsmeldungZurAufhebungEinerZukZuordnung =
            "44038";
        public const string UTILMD_44039_WimGas_KuendigungMsb = "44039";
        public const string UTILMD_44040_WimGas_BestaetigungKuendigungMsb = "44040";
        public const string UTILMD_44041_WimGas_AblehnungKuendigungMsb = "44041";
        public const string UTILMD_44042_WimGas_AnmeldungMsb = "44042";
        public const string UTILMD_44043_WimGas_BestaetigungAnmeldungMsb = "44043";
        public const string UTILMD_44044_WimGas_AblehnungAnmeldungMsb = "44044";
        public const string UTILMD_44051_WimGas_EndeMsb = "44051";
        public const string UTILMD_44052_WimGas_BestaetigungEndeMsb = "44052";
        public const string UTILMD_44053_WimGas_AblehnungEndeMsb = "44053";
        public const string UTILMD_44060_WimGas_AntwortAufDieMsbGeschaeftsdatenanfrage = "44060";
        public const string UTILMD_44096_Tsimsg_DeklarationslisteAnMgv = "44096";
        public const string UTILMD_44097_Tsimsg_DeklarationslisteAnBkv = "44097";
        public const string UTILMD_44101_NbwGas_StammdatenZurMesslokation = "44101";
        public const string UTILMD_44102_NbwGas_AktualisierteStammdatenZurMesslokation = "44102";
        public const string UTILMD_44103_NbwGas_StammdatenZurVerbrauchendenMarktlokation = "44103";
        public const string UTILMD_44104_NbwGas_AktualisierteStammdatenZurVerbrauchendenMarktlokation =
            "44104";
        public const string UTILMD_44105_NbwGas_AblehnungAufStammdatenZurVerbrauchendenMarktlokation =
            "44105";
        public const string UTILMD_44109_SdaeGas_NichtBilarelAenderungVomLfAnNb = "44109";
        public const string UTILMD_44111_SdaeGas_AntwortAufAenderungVomLfAgAnAf = "44111";
        public const string UTILMD_44112_SdaeGas_NichtBilarelAenderungVomNbAnLf = "44112";
        public const string UTILMD_44113_SdaeGas_NichtBilarelAenderungVomNbAnMsb = "44113";
        public const string UTILMD_44115_SdaeGas_AntwortAufAenderungVomNbAgAnNb = "44115";
        public const string UTILMD_44116_SdaeGas_AenderungVomMsbAnNb = "44116";
        public const string UTILMD_44117_SdaeGas_AenderungVomMsbNbAnLf = "44117";
        public const string UTILMD_44119_SdaeGas_AntwortAufAenderungVomMsbAgAnAf = "44119";
        public const string UTILMD_44120_SdaeGas_BilarelAenderungVomLfAnNb = "44120";
        public const string UTILMD_44121_SdaeGas_AntwortAufBilarelAenderungVomLfNbAnLf = "44121";
        public const string UTILMD_44123_SdaeGas_BilarelAenderungVomNbMitAbhaengigkeitenNbAnLf_Msb =
            "44123";
        public const string UTILMD_44124_SdaeGas_AntwortAufBilarelAenderungVomNbMitAbhaengigkeitenAgAnNb =
            "44124";
        public const string UTILMD_44137_SdaeGas_NichtBilarelAnfrageAnLfNbBerechtigtAnLf = "44137";
        public const string UTILMD_44138_SdaeGas_AntwortAufNichtBilarelAnfrageAnLfLfAnNb = "44138";
        public const string UTILMD_44139_SdaeGas_NichtBilarelAnfrageAnNbLfAnNb = "44139";
        public const string UTILMD_44140_SdaeGas_NichtBilarelAnfrageAnNbMsbAnNb = "44140";
        public const string UTILMD_44142_SdaeGas_AntwortAufNichtBilarelAnfrageAnNbNbAnAf = "44142";
        public const string UTILMD_44143_SdaeGas_AnfrageAnMsbMitAbhaengigkeitenLfAnNb = "44143";
        public const string UTILMD_44145_SdaeGas_AntwortAufAnfrageAnMsbMitAbhaengigkeitenNbAnAf =
            "44145";
        public const string UTILMD_44146_SdaeGas_AblehnungDerAnfrageAnMsbMitAbhaengigkeitenNbAnAf =
            "44146";
        public const string UTILMD_44147_SdaeGas_AnfrageAnMsbMitAbhaengigkeitenNbVerteilerAnMsb =
            "44147";
        public const string UTILMD_44148_SdaeGas_AnfrageAnMsbMitAbhaengigkeiten = "44148";
        public const string UTILMD_44149_SdaeGas_AntwortAufAnfrageAnMsbMitAbhaengigkeitenMsbAnNb =
            "44149";
        public const string UTILMD_44150_SdaeGas_BilarelAnfrageAnLfNbAnLf = "44150";
        public const string UTILMD_44151_SdaeGas_AntwortAufBilarelAnfrageAnLfLfAnNb = "44151";
        public const string UTILMD_44152_SdaeGas_AblehnungDerBilarelAnfrageAnLfLfAnNb = "44152";
        public const string UTILMD_44156_SdaeGas_BilarelAnfrageAnNbMitAbhaengigkeiten = "44156";
        public const string UTILMD_44157_SdaeGas_AntwortAufBilarelAnfrageAnNbMitAbhaengigkeiten =
            "44157";
        public const string UTILMD_44159_SdaeGas_AenderungVomMsbOhneAbhaengigkeitenMsbAnNb =
            "44159";
        public const string UTILMD_44160_SdaeGas_AenderungVomMsbOhneAbhaengigkeitenNbAnLf_Msb =
            "44160";
        public const string UTILMD_44161_SdaeGas_AenderungVomMsbOhneAbhaengigkeitenAgAnAf = "44161";
        public const string UTILMD_44162_Sdae_AnfrageAnMsbOhneAbhaengigkeitenVonLf_MsbAnNb =
            "44162";
        public const string UTILMD_44163_SdaeGas_AntwortAufAnfrageAnMsbOhneAbhaengigkeitenLf_MsbAnNb =
            "44163";
        public const string UTILMD_44164_SdaeGas_AblehnungAufAnfrageAnMsbOhneAbhaengigkeitenLf_MsbAnNb =
            "44164";
        public const string UTILMD_44165_SdaeGas_AnfrageAnMsbOhneAbhaengigkeitenNbVerteilerAnMsb =
            "44165";
        public const string UTILMD_44166_SdaeGas_AnfrageAnMsbOhneAbhaengigkeitenNbBerechtigterAnMsb =
            "44166";
        public const string UTILMD_44167_SdaeGas_AntwortAufAnfrageAnMsbOhneAbhaengigkeitenMsbAnNb =
            "44167";
        public const string UTILMD_44168_WimGas_Verpflichtungsanfrage_Aufforderung = "44168";
        public const string UTILMD_44169_WimGas_BestaetigungVerpflichtungsanfrage = "44169";
        public const string UTILMD_44170_WimGas_AblehnungVerpflichtungsanfrage = "44170";
        public const string UTILMD_44172_SdaeGas_AnfrageAnMsbMitAbhaengigkeitMsbAnNb = "44172";
        public const string UTILMD_44175_SdaeGas_AenderungDerMarktlokationsstrukturNbAnLf = "44175";
        public const string UTILMD_44176_SdaeGas_AntwortAufAenderungDerMarktlokationsstrukturLfAnNb =
            "44176";
        public const string UTILMD_44180_SdaeGas_AnfrageDerKomplexenMarktlokationsstrukturLfAnNb =
            "44180";
        public const string UTILMD_44181_SdaeGas_AntwortAufAnfrageDerKomplexenMarktlokationsstrukturNbAnLf =
            "44181";
        public const string UTILMD_44182_SdaeGas_AblehnungDerAnfrageDerKomplexenMarktlokationsstrukturNbAnLf =
            "44182";

        #endregion

        #region Weichenindex

        public const string WEICHENINDEX_99001_WeichenindexUpdateSelbstDefiniert = "99001";

        #endregion

        #region Marktteilnehmer

        public const string MARKTTEILNEHMER_99101_MarktteilnehmerSelbstDefiniert = "99101";

        #endregion
        #region MaloIdent

        public const string MALOIDENT_99201_Anfrage = "99201";
        public const string MALOIDENT_99202_PositiveRueckmeldung = "99202";
        public const string MALOIDENT_99203_NegativeRueckmeldung = "99203";

        #endregion

#pragma warning restore CS1591
    }
}
