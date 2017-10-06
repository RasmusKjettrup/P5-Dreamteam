# -*- coding: utf-8 -*-
import scrapy
import re
import time
from collections import deque


class BooksSpider(scrapy.Spider):
    name = 'books'
    allowed_domains = ['amazon.co.uk']
    start_urls = ['https://www.amazon.co.uk/Hobbit-J-R-Tolkien/dp/0007458428/ref=sr_1_1?s=books&ie=UTF8&qid=1507280075&sr=1-1&keywords=the+hobbit']

    def __init__(self):
        global queue
        global book_list
        global crawled_book_list
        global file_items
        global visited_product_ids
        global file_relations

        queue = deque()
        book_list = []
        crawled_book_list = []
        visited_product_ids = ['/dp/0007458428/']

        file_items = open('items.txt', 'w')
        file_items.write('')
        file_items = open('items.txt', 'a')

        file_relations = open('relations.txt', 'w')
        file_relations.write('')
        file_relations = open('relations.txt', 'a')

    def parse(self,response):

        def next_request(self):
            next_link = queue.popleft()
            return scrapy.Request(next_link, callback=self.parse)

        def add_relation(book_one, book_two):

            def add_book(book):
                file_items.write(str(len(book_list))+", "+book+"\n")
                book_list.append(book)

            if not(book_one in book_list):
                add_book(book_one)

            if not(book_two in book_list):
                add_book(book_two)

            file_relations.write(str(book_list.index(book_one))+", "+str(book_list.index(book_two))+"\n")
            return

        time.sleep(1)
        book_response = response.xpath('//*[@id="productTitle"]/text()').extract_first()
        if book_response == None:
            print('            Title not found!')
            result = next_request(self)
            yield result
            return
        booktitle = book_response.strip()
        if (booktitle in crawled_book_list):
            result = next_request(self)
            yield result
            return
        crawled_book_list.append(booktitle)
        print(booktitle+" ("+str(len(queue))+")")

        book_regex = re.compile('(Paperback|Hardcover)')
        formats = response.xpath('//div[@id="tmmSwatches"]').extract_first()
        if (formats == None) and not (book_regex.search(formats)):
            print('            Not a book!')
            result = next_request(self)
            yield result
            return

        links = response.css('li.a-carousel-card > div:first-child > a:first-child::attr(href)').extract()[:6]
        names = response.css('li.a-carousel-card > div:first-child > a:first-child > div:nth-child(2)::text').extract()[:6]

        for i in range(len(names)):
            names[i] = names[i].strip()

        if (len(links) != len(names)):
            print("FAULT: Amount of links does not equal amount of names ("+ str(len(links))+" "+str(len(names))+")")

        id_regex = re.compile('/dp/[0-9A-Z]+/')
        nameCount = 0
        for link in links:
            new_link = response.urljoin(link)

            id_result = id_regex.search(new_link)
            if not id_result:
                print("FAULT: "+new_link+" did not match id_regex")
                continue

            new_link_product_id = id_result.group(0)
            if not(new_link_product_id in visited_product_ids):
                queue.append(response.urljoin(link))
                visited_product_ids.append(new_link_product_id)
            
            add_relation(booktitle, names[nameCount])
            nameCount += 1

        result = next_request(self)
        yield result