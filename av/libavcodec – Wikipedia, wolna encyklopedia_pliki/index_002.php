/*
USAGE:
On en.wikipedia, add the following line:
 
if ( mw.config.get( 'wgCanonicalSpecialPageName' ) === 'Search' ||  ( mw.config.get( 'wgArticleId' ) === 0 && mw.config.get( 'wgCanonicalSpecialPageName' ) === false ) ) {
	importScript('MediaWiki:Wdsearch.js');
}

to your [[Special:Mypage/common.js|common.js]] page. On other Wikipedias, add

if ( mw.config.get( 'wgCanonicalSpecialPageName' ) === 'Search' ||  ( mw.config.get( 'wgArticleId' ) === 0 && mw.config.get( 'wgCanonicalSpecialPageName' ) === false ) ) {
	importScriptURI("//en.wikipedia.org/w/index.php?title=MediaWiki:Wdsearch.js&action=raw&ctype=text/javascript");
}

instead. To change the header line to your language, have an admin add the appropriate line to this page.
 
*/
 
var prevent_wd_auto_desc = true ; // No auto-run
 
$(document).ready ( function () {
	var testing = false ;
	if ( testing ) console.log ( "Initiating WDsearch") ;
	if ( mw.config.get( 'wgCanonicalSpecialPageName' ) !== 'Search' && mw.config.get( 'wgArticleId' ) !== 0 ) return;
	var mode = 'searchresults' ;
	var results = $('div.searchresults') ;
	if ( results.length == 0 ) {
		mode = 'noarticletext' ;
		results = $('div.noarticletext table') ;
	}
 
	if ( results.length == 0 ) return ; // No search results, no search page. Bye.
 
	mw.loader.load( ['jquery.ui.dialog'] );
	importScriptURI("//en.wikipedia.org/w/index.php?title=MediaWiki:Wdsearch-autodesc.js&action=raw&ctype=text/javascript");
 
	var i18n = {
		'en' : {
			'commons_cat' : 'Commons category' ,
			'wikipedias' : 'Wikipedia articles' ,
			'header' : 'Wikidata search results' ,
			'reasonator' : 'Show item details on Reasonator'
		},
		'bn' : {
			'commons_cat' : 'কমন্স বিষয়শ্রেণী' ,
			'wikipedias' : 'উইকিপিডিয়া নিবন্ধ' ,
			'header' : 'উইকিউপাত্ত অনুসন্ধানের ফলাফল',
			'reasonator' : 'Reasonator-এ আইটেমের বিস্তারিত দেখাও'
		},
		'de' : {
			'commons_cat' : 'Kategorie auf Commons' ,
			'wikipedias' : 'Wikipedia-Artikel' ,
			'header' : 'Wikidata-Suchergebnisse'
		},
		'el' : {
			'commons_cat' : 'Κατηγορία στα Commons' ,
			'wikipedias' : 'Λήμματα στη Βικιπαίδεια' ,
			'header' : 'Αποτελέσματα αναζήτησης στα Wikidata'
		},
		'eo' : {
			'commons_cat' : 'Komuneja kategorio' ,
			'wikipedias' : 'Vikipediaj artikoloj' ,
			'header' : 'Serĉorezultoj de Vikidatumoj' ,
			'reasonator' : 'Montri detalojn en Reasonator'
		},
		'es' : {
			'commons_cat' : 'Categoría en Commons' ,
			'wikipedias' : 'Artículos en Wikipedia' ,
			'header' : 'Resultados de la búsqueda en Wikidata' ,
			'reasonator' : 'Mostrar los detalles en Reasonator'
		},
		'eu' : {
			'commons_cat' : 'Commonseko kategoria' ,
			'wikipedias' : 'Wikipediako artikuluak' ,
			'header' : 'Wikidatako bilaketaren emaitzak' ,
			'reasonator' : 'Erakutsi Reasonatorreko xehetasunak'
		},
		'he' : {
			'commons_cat': 'קטגוריית ויקישיתוף',
			'wikipedias': 'ערכים בוויקיפדיה',
			'header': 'תוצאות חיפוש בוויקינתונים'
		},
		'id' : {
			'commons_cat' : 'Kategori Commons' ,
			'wikipedias' : 'Artikel Wikipedia' ,
			'header' : 'Hasil pencarian Wikidata' ,
			'reasonator' : 'Tunjukkan detil item di Reasonator'
		},
		'ilo' : {
			'commons_cat' : 'Kategoria ti Commons' ,
			'wikipedias' : 'Dagiti artikulo ti Wikipedia' ,
			'header' : 'Dagiti resulta ti panagbiruk iti Wikidata' ,
			'reasonator' : 'Ipakita dagiti salaysay ti banag iti Reasonator'
		},
		'it' : {
			'commons_cat' : 'Categoria in Commons' ,
			'wikipedias' : 'Voci di Wikipedia' ,
			'header' : 'Risultati da Wikidata'
		},
		'fr' : {
			'commons_cat' : 'Catégorie sur Commons' ,
			'wikipedias' : 'Articles sur Wikipédia' ,
			'header' : 'Résultats sur Wikidata'
		},
		'pl' : {
			'commons_cat' : 'Kategoria na Commons' ,
			'wikipedias' : 'Artykuły w Wikipedii',
			'header' : 'Wyniki wyszukiwania w Wikidata' ,
			'reasonator' : 'Pokaż szczegóły w Reasonatorze'
		},
		'pt' : {
			'commons_cat' : 'Categoria no Commons' ,
			'wikipedias' : 'Artigos da Wikipédia' ,
			'header' : 'Resultados da busca no Wikidata'
		},
		'pt-br' : {
			'commons_cat' : 'Categoria no Commons' ,
			'wikipedias' : 'Artigos da Wikipédia' ,
			'header' : 'Resultados da busca no Wikidata'
		},
                'ru' : {
			'commons_cat' : 'Категория Викисклада' ,
			'wikipedias' : 'Статьи Википедии' ,
			'header' : 'Результаты поиска в Викиданных' ,
			'reasonator' : 'Посмотреть подробности через Reasonator'
		},
		'sk' : {
			'commons_cat' : 'Kategória na Commons' ,
			'wikipedias' : 'Články na Wikipédii' ,
			'header' : 'Výsledky hľadania na Wikiúdajoch' ,
			'reasonator' : 'Zobraziť podrobnosti v Reasonatore'
		},
		'sv' : {
			'commons_cat' : 'Kategorier på Commons' ,
			'wikipedias' : 'Wikipediaartiklar' ,
			'header' : 'Sökresultat från Wikidata'
		},
		'udm' : {
			'commons_cat' :	'Викискладысь категория' ,
			'wikipedias' : 'Википедиысь статьяос' ,
			'header' : 'Викиданнойёсысь утчанлэн йылпумъянъёсыз' ,
			'reasonator' : 'Тыро-пыдогес учконо Reasonator пыр'
		},
		'zh' : {
			'commons_cat' : '维基共享资源类别' ,
			'wikipedias' : '维基百科文章' ,
			'header' : '维基数据搜索结果' ,
			'reasonator' : '上Reasonator显示项目的详细信息'
		}
	} ;
	var i18n_lang = wgUserLanguage ;
	if ( undefined === i18n[i18n_lang] ) i18n_lang = 'en' ; // Fallback
 
	if ( testing ) console.log ( "Preparing WDsearch" ) ;
 
	var api = '//www.wikidata.org/w/api.php?callback=?' ;
 
	function run () {
 
		if ( testing ) console.log ( "Trying to run WDsearch") ;
 
		if ( typeof(wd_auto_desc) == 'undefined' ) {
			setTimeout ( run , 100 ) ;
			return ;
		}
 
		if ( testing ) console.log ( "Running WDsearch") ;
 
		wd_auto_desc.lang = wgUserLanguage ;
 
		var query ;
		if ( mode == 'searchresults' ) {
			query = $('#powerSearchText').val() ;
			if ( $('#powerSearchText').length == 0 ) query = $('#searchText').val() ;
		} else if ( mode == 'noarticletext' ) query = wgPageName ;
 
		if ( testing ) console.log ( "Using mode " + mode + " and query :" + query ) ;
 
		$.getJSON ( api , {
			action:'query',
			list:'search',
			srsearch:query,
			srnamespace:0,
			format:'json'
		} , function (d) {
			if ( testing ) console.log(d);
			if ( undefined === d.query || undefined === d.query.search || d.query.search.length == 0 ) return ; // No result
 
			var ids = [] ;
			var q = [] ;
			var h = "<div id='wdsearch_container'>" ;
			h += '<h3>' ;
			h += i18n[i18n_lang].header ;
			h += '</h3><table><tbody>' ;
			$.each ( d.query.search , function ( k , v ) {
				q.push ( v.title ) ;
				var title = [] ;
				var snip = $('<span>'+v.snippet+'</span>') ;
				$.each ( snip.find('span.searchmatch') , function ( a , b ) {
					var txt = $(b).text() ;
					if ( -1 != $.inArray ( txt , title ) ) return ;
					title.push ( txt ) ;
				} )
				if ( title.length == 0 ) title = [ v.title ] ; // Fallback to Q
				ids.push ( v.title ) ;
				h += "<tr id='" + v.title + "'>" ;
				h += "<th><a class='wd_title' href='//www.wikidata.org/wiki/" + v.title + "'>" + title.join ( ' ' ) + "</a></th>" ;
				h += "<td><span class='wd_desc'></span><span class='wd_manual_desc'></span></td>" ;
				h += "<td>" ;
				h += "<span class='wikipedias'></span>" ;
				h += "<span class='commonscat'></span>" ;
				var rs = i18n[i18n_lang].reasonator ;
				if ( rs === undefined ) rs = i18n['en'].reasonator ;
				h += "<a title='"+rs+"' href='//tools.wmflabs.org/reasonator/?lang="+wgUserLanguage+"&q="+v.title+"'><img src='//upload.wikimedia.org/wikipedia/commons/thumb/e/e8/Reasonator_logo_proposal.png/16px-Reasonator_logo_proposal.png' border=0/></a>" ;
				h += "</td>" ;
				h += "</tr>" ;
			})
			h += "</tbody></table>" ;
			h += "</div>" ;
 
 
			if ( mode == 'searchresults' ) {
				$('#mw-content-text').append ( h ) ;
			} else if ( mode == 'noarticletext' ) {
				$('div.noarticletext').after ( h ) ;
			}
 
			if ( ids.length == 0 ) return ;
 
			$.getJSON ( api , {
				action:'wbgetentities',
				ids:ids.join('|'),
				format:'json',
				languages:wgUserLanguage
			} , function ( d ) {
				if ( d === undefined || d.entities === undefined ) return ; // Some error
				$.each ( d.entities , function ( q , v ) {
 
					if ( v.claims && undefined !== v.claims['P373'] ) { // Commons cat
						var cat = v.claims['P373'][0].mainsnak.datavalue.value ;
						var h = " <a title='"+i18n[i18n_lang].commons_cat+"' href='//commons.wikimedia.org/wiki/Category:"+escape(cat)+"'><img src='https://upload.wikimedia.org/wikipedia/commons/thumb/4/4a/Commons-logo.svg/12px-Commons-logo.svg.png' border=0 /></a>" ;
						$('#'+q+' span.commonscat').html ( h ) ;
					}
 
					if ( undefined !== v.labels && undefined !== v.labels[wgUserLanguage] ) { // Label
						var h = v.labels[wgUserLanguage].value ;
						$('#'+q+' a.wd_title').html ( h ) ;
					}
 
					if ( undefined !== v.descriptions && undefined !== v.descriptions[wgUserLanguage] ) { // Manual desc
						var h = "; " + v.descriptions[wgUserLanguage].value ;
						$('#'+q+' span.wd_manual_desc').html ( h ) ;
					}
 
					if ( undefined !== v.sitelinks ) { // Wikipedia links
						var wikipedias = [] ;
						$.each ( v.sitelinks , function ( site , v2 ) {
							var m = site.match ( /^(.+)wiki$/ ) ;
							if ( null == m  ) return ; // Wikipedia only
							wikipedias.push ( { site:site , title:v2.title , url:'//'+m[1]+'.wikipedia.org/wiki/'+escape(v2.title) } ) ;
						} ) ;
						if ( wikipedias.length > 0 ) {
							var h = " <a title='"+i18n[i18n_lang].wikipedias+"' href='#'><img src='https://upload.wikimedia.org/wikipedia/commons/thumb/8/80/Wikipedia-logo-v2.svg/14px-Wikipedia-logo-v2.svg.png' border=0 /></a>" ;
							$('#'+q+' span.wikipedias').html ( h ) ;
							$('#'+q+' span.wikipedias a').click ( function () {
								var did = 'wdsearch_dialog' ;
								$('#'+did).remove() ; // Cleanup
								var h = "<div title='"+i18n[i18n_lang].wikipedias+"' id='"+did+"'><div style='overflow:auto'>" ;
								h += "<table class='table table-condensed table-striped'>" ;
								h += "<thead><tr><th>Site</th><th>Page</tr></thead><tbody>" ;
								$.each ( wikipedias , function ( k , v3 ) {
									h += "<tr><td>" + v3.site + "</td><td>" ;
									h += "<a href='"+v3.url+"'>" + v3.title + "</a>" ;
									h += "</td></tr>" ;
								} ) ;
								h += "</tbody></table>" ;
								h += "</div></div>" ;
								$('#wdsearch_container').prepend ( h ) ;
								$('#'+did).dialog ( {
									modal:true
								} )
								return false ;
							} ) ;
						}
					}
 
				} ) ;
			} ) ;
 
			var the_project = wgSiteName.toLowerCase() ;
			if ( the_project == "wikimedia commons" ) the_project = 'wikipedia' ;
			$.each ( q , function ( k , v ) {
				wd_auto_desc.loadItem ( v , {
					target:$('#'+v+' span.wd_desc') ,
					links : the_project ,
//					callback : function ( q , html , opt ) { console.log ( q + ' : ' + html ) } ,
//					linktarget : '_blank'
				} ) ;
			})
 
		})
	}
 
	run() ;
})