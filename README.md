# BookBarn

The Book Barn is where you go to find your next read! This repo contains the various components and services to scrape book information from different sources (initially GoodReads) and provide ways to access the aggregated book data.

This is primarily a practice and demonstration project for building a robust and scalable architecture (but also a little useful since GoodReads's browsing and search functionality is really bad).

# Components

## Common
- BookBarn.Model - Common objects and interfaces used across the project.
- BookBarn.Instrumentation - TBD - common metadata and implementation for instrumenting BookBarn components.
  
## Crawler
- BookBarn.Crawler - Generalized framework for defining crawling and parsing logic and executing
- BookBarn.Crawler.GoodReads - Crawler impmenentations for scraping content on GoodReads.com
- BookBarn.Crawler.Host - Host service for the crawler framework/implementations
- BookBarn.Crawler.Queue - Distributed queue implementation for crawler host instances to use.

## API
- BookBarn.Api - The host service for the BookBarn data and media stores. Hydrated by the Crawler components, consumed by the frontend components
- BookBarn.Api.Client - Client library for consuming the API service. 

## Frontend
TBD
