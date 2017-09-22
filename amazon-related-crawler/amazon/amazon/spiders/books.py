# -*- coding: utf-8 -*-
import scrapy
import re


class BooksSpider(scrapy.Spider):
    name = 'books'
    allowed_domains = ['amazon.com']
    start_urls = ['https://www.amazon.com/gp/product/1787300080/']
    
    def __init__(self):
        global l 
        global i
        l = []
        i = 0
    
    def parse(self, response):
        global l
        global i
        booktitle = response.xpath('//*[@id="productTitle"]/text()').extract_first().strip()
        print(booktitle)

        regex = re.compile('(Paperback|Hardcover)')
        formats = response.xpath('//div[@id="tmmSwatches"]').extract_first()
        result = regex.search(formats)

        if not result:
            print('            Not a book!')
            return

        links = response.css('li.a-carousel-card > div:first-child > a:first-child::attr(href)').extract()
        #print(links)
        for link in links:
            next_page = response.urljoin(link)
            l.append((booktitle, response))
            #print(l[i][0])
            i += 1
            yield scrapy.Request(next_page, callback=self.parse)
