document.itxtDebugOn=0;if('undefined'==typeof $iTXT){$iTXT={};};$iTXT.debug={Log:function()
{},Category:{},error:function()
{},info:function()
{},debug:function()
{},trace:function()
{},Util:{isLoggingOn:function()
{return false;},hilite:function()
{}}}
itxtFeedback=function()
{};
if('undefined'==typeof $iTXT){$iTXT={};};document.itxtDisabled=1;
if(document.itxtDisabled)
{document.itxtInProg=1;
if(!$iTXT.cnst){$iTXT.cnst={};}
if(!$iTXT.debug){$iTXT.debug={};}
if(!$iTXT.glob){$iTXT.glob={track:{}};}
if(!$iTXT.js){$iTXT.js={};}
if(!$iTXT.tmpl){$iTXT.tmpl={};}
if(!$iTXT.tmpl.js){$iTXT.tmpl.js={};}
if(!$iTXT.tmpl.components){$iTXT.tmpl.components={};}
if(!$iTXT.core){$iTXT.core={};};

if(!$iTXT.data){$iTXT.data={};};

if(!$iTXT.debug){$iTXT.debug={};};


if(!$iTXT.fx){$iTXT.fx={};};

if(!$iTXT.fx2){$iTXT.fx2={};};

if(!$iTXT.itxt){$iTXT.itxt={};};

if(!$iTXT.metrics){$iTXT.metrics={};};

if(!$iTXT.tmpl){$iTXT.tmpl={};};

if(!$iTXT.ui){$iTXT.ui={};};

if(!$iTXT.ui_mobile){$iTXT.ui_mobile={};};


document.itxtIsReady=0;
$iTXT.js.loaderCallbacks=[];$iTXT.js.exclCont=function()
{try
{var d=document.getElementById('itxtexclude');if(null==d)
{var b=document.getElementsByTagName('body')[0];d=document.createElement('div');d.id='itxtexclude';b.insertBefore(d,b.firstChild);}
return d;}catch(x){};};$iTXT.js.load=function(src)
{if('string'!=typeof src||(!src.match(/^http/)&&!src.match(/^file/)))
{return;};try
{var e=document.createElement('script');e.src=src;e.type='text/javascript';var d=$iTXT.js.exclCont();d.insertBefore(e,d.firstChild);}catch(x){};};$iTXT.js.loadCss=function(src,id){try
{var ss=document.createElement('link');ss.id=id;ss.href=src;ss.type='text/css';ss.rel='stylesheet';var d=$iTXT.js.exclCont();d.insertBefore(ss,d.firstChild);}catch(x){}};if(!$iTXT.js.loader){$iTXT.js.loader={};}
$iTXT.js.libPath='http://images.intellitxt.com/ast/js/vm/jslib/';$iTXT.js.loadLib=function(libName,className)
{var lib='$iTXT.'+libName+'.'+className;var path=$iTXT.js.libPath+libName+'/'+className.toLowerCase()+'.js';if('undefined'==typeof($iTXT.js.loader[lib]))
{$iTXT.js.loader[lib]=false;};};$iTXT.js.check=function()
{if(!document.itxtIsReady)
{return window.setTimeout($iTXT.js.check,100);}
var error=0;for(var libName in $iTXT.js.loader)
{if(!$iTXT.js.loader[libName])
{error=1;break;};}
if(error)
{window.setTimeout($iTXT.js.check,100);}
else
{var currentLibName='Unkown';try
{for(var libName in $iTXT.js.loader)
{currentLibName=libName;eval(libName+'_Load()');}}
catch(e)
{}
$iTXT.js.librariesLoaded=true;$iTXT.core.$(document).itxtFire('$iTXT:js:load');for(var i=0;i<$iTXT.js.loaderCallbacks.length;i++)
{$iTXT.js.loaderCallbacks[i]();}}};
if(!$iTXT.tmpl.loader){$iTXT.tmpl.loader={};}
$iTXT.tmpl.versions={'madt_pricegrabber':'1329390699',
'madc_pricegrabberfooter':'1329390699',
'madt_freeform':'1355425625',
'madt_lightbox':'1382372837',
'madt_businessdotcom':'1329390699',
'madt_dynamicadhesion':'1383069954',
'madt_lightboxmosaic':'1385047768',
'madc_aura2pedestal':'1350998192',
'madt_dynamicadhesionyell':'1381851764',
'madt_backfill':'1334158639',
'madt_lightboxproduct':'1385047768',
'madc_backfilllist':'1366643353',
'madt_relatedcontent':'1357570634',
'madt_billboard':'1329390699',
'madt_yell':'1374605568',
'madt_genericvcs':'1329390699',
'madc_relatedcontentlist':'1366643353',
'madt_freeformwithfooter':'1359111685',
'madc_persistent':'1384439282',
'madt_bing':'1329390699',
'madc_adrepeater':'1366643353',
'madt_backfillmultiimage':'1366208374',
'madt_livelookup':'1308735381',
'madc_searchbar':'1329390699',
'madt_ebay':'1329390699',
'madt_valueclick':'1358272719',
'madt_shopping':'1363190221',
'madt_expandableflash':'1351256125',
'madc_advertfooter':'1359111685',
'madt_websearchdrawer':'1329390699',
'madc_progressbar':'1380635370',
'madt_become':'1329390699',
'madt_html5video':'1309531585',
'madt_aura2':'1369759724',
'madc_progressbartail':'1358941244',
'madt_bingrc':'1342798303',
'madc_progressbarheader':'1359115078',
'madc_expandableunit':'1347031349',
'madc_progressbarfooter':'1358941246',
'madt_genericflash':'1352367151',
'madt_dynamicadhesionbackfill':'1383069953',
'madt_dynamicadhesionshopping':'1382017905',
'madt_yahoo':'1358272719',
'madt_dynamicadhesionyahoo':'1381851764',
'madt_lightboxmobile':'1376058616',
'madt_generic':'1378735942',
'madt_pricerunner':'1333973419',
'madc_aura2header':'1358941245'};$iTXT.tmpl.js.loadPath='http://images.intellitxt.com/ast/js/vm/jslib/templates/';$iTXT.tmpl.components.loadPath='http://images.intellitxt.com/ast/js/vm/jslib/components/';$iTXT.tmpl.load=function(name,isComp)
{var lib='$iTXT.tmpl.js.'+name;var version=$iTXT.tmpl.versions['madt_'+name.toLowerCase()]||'';if(version!='')
{version='_'+version;};var path=$iTXT.tmpl.js.loadPath+name.toLowerCase()+version+'.js';if(isComp)
{lib='$iTXT.tmpl.components.'+name;version=$iTXT.tmpl.versions['madc_'+name.toLowerCase()]||'';if(version!='')
{version='_'+version;}
path=$iTXT.tmpl.components.loadPath+name.toLowerCase()+version+'.js';};if('undefined'==typeof($iTXT.tmpl.loader[lib]))
{$iTXT.tmpl.loader[lib]=false;$iTXT.js.load(path);};};$iTXT.tmpl.dependsOn=function(name,isComp)
{$iTXT.tmpl.load(name,isComp);if(!$iTXT.tmpl.checkInProgress)
{$iTXT.tmpl.check();}};$iTXT.tmpl.check=function()
{$iTXT.tmpl.checkInProgress=true;var error=0;for(var libName in $iTXT.tmpl.loader)
{if(!$iTXT.tmpl.loader[libName])
{error=1;};};if(error)
{window.setTimeout($iTXT.tmpl.check,100);}
else
{try
{for(var libName in $iTXT.tmpl.loader)
{eval(libName+'_Load()');}}
catch(e)
{}
$iTXT.tmpl.checkInProgress=false;$iTXT.core.$(document).itxtFire('$iTXT:tmpl:load');}};
$iTXT.js.pageLoaded=(document.readyState==='complete');document.itxtOnLoad=function()
{$iTXT.js.pageLoaded=true;if(!document.referrer||document.location.href.indexOf(document.referrer)!=0)
{document.itxtReady();}};document.itxtReady=function()
{if(document.itxtIsReady)
{return;};if($iTXT.js.qaol&&!$iTXT.js.pageLoaded)
{return;};document.itxtIsReady=true;if(document.itxtLoadLibraries)
{document.itxtLoadLibraries();};$iTXT.js.loadCss('http://images.intellitxt.com/ast/js/vm/style/itxtcss_1379667726.css','itxtcss');$iTXT.cnst.CSS_DIR='http://images.intellitxt.com/ast/js/vm/style/';$iTXT.cnst.CSS_VER='1379667726';};document.itxtDOMContentLoaded=function()
{document.removeEventListener('DOMContentLoaded',document.itxtDOMContentLoaded,false);document.itxtReady();};document.itxtDOMContentLoadedIE=function()
{if(document.readyState==='complete')
{document.detachEvent('onreadystatechange',document.itxtDOMContentLoadedIE);document.itxtReady();};};if(document.addEventListener)
{window.addEventListener('load',document.itxtOnLoad,false);}
else if(document.attachEvent)
{window.attachEvent('onload',document.itxtOnLoad,false);};if(document.readyState==='complete'||document.readyState==='interactive')
{document.itxtReady();}
else
{if(document.addEventListener)
{document.addEventListener('DOMContentLoaded',document.itxtDOMContentLoaded,false);}
else if(document.attachEvent)
{document.attachEvent('onreadystatechange',document.itxtDOMContentLoadedIE,false);itxtIEDoScroll();};};function itxtIEDoScroll()
{if(document.itxtIsReady)
{return;};try
{document.documentElement.doScroll('left');}
catch(e)
{setTimeout(itxtIEDoScroll,1);return;};document.itxtReady();};if(typeof document.readyState==='undefined')
{var itxtGetLastElmt=function()
{var elms=document.getElementsByTagName('*');return elms[elms.length-1];};var itxtLastElmt=itxtGetLastElmt();setTimeout(function()
{if(itxtGetLastElmt()===itxtLastElmt&&typeof document.readyState==='undefined')
{document.itxtReady();}},1000);};if(1===document.itxtPostDomLoad)
{document.itxtReady();};
document.itxtLoadLibraries=function()
{if(!document.itxtLibrariesLoading)
{document.itxtLibrariesLoading=true;
$iTXT.js.load($iTXT.js.libPath+'resources/underscore-min-ns-1.4.2.js');
$iTXT.js.loadLib('core','Util');
$iTXT.js.loadLib('core','Builder');
$iTXT.js.loadLib('core','Browser');
$iTXT.js.loadLib('core','Class');
$iTXT.js.loadLib('core','Dom');
$iTXT.js.loadLib('core','Event');
$iTXT.js.loadLib('core','Flash');
$iTXT.js.loadLib('core','Math');
$iTXT.js.loadLib('core','Array');
$iTXT.js.loadLib('core','Ajax');
$iTXT.js.loadLib('core','Regex');
$iTXT.js.load($iTXT.js.libPath+'core_1382536355.js');

$iTXT.js.loadLib('data','AdLogger');
$iTXT.js.loadLib('data','Advert');
$iTXT.js.loadLib('data','AdvertHandler');
$iTXT.js.loadLib('data','Dom');
$iTXT.js.loadLib('data','Context');
$iTXT.js.loadLib('data','Country');
$iTXT.js.loadLib('data','Param');
$iTXT.js.loadLib('data','Pixel');
$iTXT.js.loadLib('data','Channel');
$iTXT.js.load($iTXT.js.libPath+'data_1386616211.js');

$iTXT.js.loadLib('debug','Util');
$iTXT.js.load($iTXT.js.libPath+'debug_1386616209.js');


$iTXT.js.loadLib('fx','Base');
$iTXT.js.loadLib('fx','Combination');
$iTXT.js.loadLib('fx','Fade');
$iTXT.js.loadLib('fx','Move');
$iTXT.js.loadLib('fx','Queue');
$iTXT.js.loadLib('fx','Size');
$iTXT.js.load($iTXT.js.libPath+'fx_1352896232.js');

$iTXT.js.loadLib('fx2','Fade2');
$iTXT.js.loadLib('fx2','Tweens');
$iTXT.js.load($iTXT.js.libPath+'fx2_1353596579.js');

$iTXT.js.loadLib('itxt','Controller');
$iTXT.js.load($iTXT.js.libPath+'itxt_1382536355.js');

$iTXT.js.loadLib('metrics','Events');
$iTXT.js.load($iTXT.js.libPath+'metrics_1329390699.js');

$iTXT.js.loadLib('tmpl','ElementBase');
$iTXT.js.loadLib('tmpl','TemplateBase');
$iTXT.js.loadLib('tmpl','Cell');
$iTXT.js.loadLib('tmpl','Flash');
$iTXT.js.loadLib('tmpl','Iframe');
$iTXT.js.loadLib('tmpl','Image');
$iTXT.js.loadLib('tmpl','Input');
$iTXT.js.loadLib('tmpl','Link');
$iTXT.js.loadLib('tmpl','Row');
$iTXT.js.loadLib('tmpl','Text');
$iTXT.js.loadLib('tmpl','Html');
$iTXT.js.loadLib('tmpl','Html5Video');
$iTXT.js.load($iTXT.js.libPath+'tmpl_1384879292.js');

$iTXT.js.loadLib('ui','AutoPeek');
$iTXT.js.loadLib('ui','ComponentBase');
$iTXT.js.loadLib('ui','Hook');
$iTXT.js.loadLib('ui','Tooltip');
$iTXT.js.loadLib('ui','TooltipChrome');
$iTXT.js.loadLib('ui','TooltipContent');
$iTXT.js.loadLib('ui','TooltipPlacer');
$iTXT.js.loadLib('ui','TooltipFooter');
$iTXT.js.loadLib('ui','TooltipHeader');
$iTXT.js.loadLib('ui','TooltipTail');
$iTXT.js.loadLib('ui','LightboxChrome');
$iTXT.js.loadLib('ui','Aura2TooltipPlacer');
$iTXT.js.loadLib('ui','Mobt');
$iTXT.js.loadLib('ui','MobtManager');
$iTXT.js.loadLib('ui','OldTooltipHeader');
$iTXT.js.load($iTXT.js.libPath+'ui_1386774048.js');

$iTXT.js.loadLib('ui_mobile','Tooltip');
$iTXT.js.loadLib('ui_mobile','TooltipChrome');
$iTXT.js.loadLib('ui_mobile','TooltipHeader');
$iTXT.js.load($iTXT.js.libPath+'ui_mobile_1362652240.js');


$iTXT.js.check();};};
$iTXT.cnst={'CONTROLLER_CONTEXTUALIZER':"/v4/context",
'WEIGHTING_DEFAULT_TEMPLATE':20,
'WEIGHTING_DEFAULT_DEBUG':70,
'IFRAME_SCRIPT_DROPPER_LOC':"iframescript.jsp",
'WEIGHTING_DEFAULT_DATABASE':40,
'IFRAME_SCRIPT_DROPPER_FLD':"src",
'DNS_SMARTAD_MARKER':".smarttargetting.",
'WEIGHTING_DEFAULT_CHANNEL':50,
'CONTROLLER_LOOK':"/v4/look",
'WEIGHTING_DEFAULT_COMPONENT':10,
'WEIGHTING_DEFAULT_TRANSLATION':30,
'Source':{"ADFTR":25,"COMPARE":13,"ICON":21,"IE":9,"IEB":100,"IEC":199,"ITXT":0,"ITXT_TST":4,"KW":10,"LOGO":12,"MULTI":14,"SA":8,"SCHINP":23,"SCHRES":24,"TT":11},
'WEIGHTING_DEFAULT_CONFIG':35,
'PIXEL_SERVER_PREFIX':"pixel",
'CONTROLLER_INITIALISER':"/v4/init",
'Params':{"UID":"pvu","REF_MD5":"sid","TIMESTAMP":"ts","FLASH_AUDIO_FLAG":"fao","REF":"refurl","UID_MD5":"pvm"},
'CONTROLLER_ADVERTISER':"/v4/advert",
'CONTROLLER_DEMOGEN':"/v4/demogen",
'CONTROLLER_LOADER':"/v4/load",
'WEIGHTING_DEFAULT_CAMPAIGN':60,
'WEIGHTING_DEFAULT_DEFAULT':0,
'DNS_INTELLITXT_SUFFIX':".intellitxt.com",
'CONTROLLER_CHUNK':"/v4/chunk"};$iTXT.js.SearchEngineSettings={'fields.bing':"q",
'ids.google':"3",
'fields.live':"q",
'ids.live':"14",
'fields.aol':"query,as_q,q",
'ids.aol':"10",
'ids.yahoo':"1",
'hosts':"yahoo,google,aol,ask,live,bing",
'ids.bing':"14",
'fields.ask':"q",
'ids.ask':"12",
'fields.google':"q,as_q",
'fields.yahoo':"p"};$iTXT.glob.itxtRunning=1;$iTXT.js.qaol=false;$iTXT.js.serverUrl='http://freecodecs.us.intellitxt.com';$iTXT.js.serverName='freecodecs.us.intellitxt.com';$iTXT.js.pageQuery='IPID=623&MK=10&FG=red';$iTXT.js.ipid='623';$iTXT.js.umat=true;$iTXT.js.startTime=(new Date()).getTime();$iTXT.js.timeout={time:-1,monitoring:false,abort:false};(function(){var e=document.createElement("img");e.src="http://b.scorecardresearch.com/b?c1=8&c2=6000002&c3=90000&c4=&c5=&c6=&c15=&cv=1.3&cj=1&rn=20140103150554";})();
if(document.itxtIsReady)
{document.itxtLoadLibraries();};};