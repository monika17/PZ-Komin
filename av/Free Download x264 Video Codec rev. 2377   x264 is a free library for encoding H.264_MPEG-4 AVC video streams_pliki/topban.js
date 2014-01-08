<!-- Original:  Rich Galichon (rich@galichon.net) -->
<!-- Free-Codecs.com -->
<!-- Begin
function banner(imgSource,url,alt,chance) {
this.imgSource = imgSource;
this.url = url;
this.alt = alt;
this.chance = chance;
}
function dispBanner() {
with (this) document.write("<A HREF=" + url + "><IMG SRC='" + imgSource + "' WIDTH=468 HEIGHT=60 BORDER=0 ALT='" + alt + "'></A>");
}
banner.prototype.dispBanner = dispBanner;
banners = new Array();
banners[0] = new banner("http://www.free-codecs.com/pictures/ban/clonedvd.gif",
                        "http://esd.element5.com/product.html?productid=540060&affiliateid=71258 target='_blank'",
                        "Get CloneDVD!",
                        5);
banners[1] = new banner("http://www.codecsdownload.com/pictures/ban/pcnot468x60b.gif",
                        "http://partners.webmasterplan.com/click.asp?ref=180266&site=3115&type=b3&bnb=3 target='_blank'",
                        "PC-Notdienst Verzeichnis für Deutschland",
                        0);
banners[2] = new banner("http://www.free-codecs.com/pictures/ban/468x60.gif",
                        "http://partners.webmasterplan.com/click.asp?ref=180266&site=2614&type=b4&bnb=4 target='_blank'",
                        "Top-Produkte verschiedener Internet-Shops",
                        0);


sum_of_all_chances = 0;
for (i = 0; i < banners.length; i++) {
sum_of_all_chances += banners[i].chance;
}
function randomBanner() {
chance_limit = 0;
randomly_selected_chance = Math.round((sum_of_all_chances - 1) * Math.random()) + 1;
for (i = 0; i < banners.length; i++) {
chance_limit += banners[i].chance;
if (randomly_selected_chance <= chance_limit) {
document.write("<A HREF=" + banners[i].url + "><IMG SRC='" + banners[i].imgSource + "' WIDTH=468 HEIGHT=60 BORDER=0 ALT='" + banners[i].alt + "'></A>");
return banners[i];
break;
      }
   }
}
//  End -->