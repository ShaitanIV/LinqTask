// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
	[Title("LINQ Module")]
	[Prefix("Linq")]
	public class LinqSamples : SampleHarness
	{

		private DataSource dataSource = new DataSource();

		[Category("Restriction Operators")]
		[Title("Where - Task 1")]
		[Description("List of clients, which total revenue is greater than condition value")]
		public void Linq1()
		{
			var condition = 1000;
			var result = dataSource.Customers.Where(x => x.Orders.Sum(y => y.Total) > condition).ToList();

			foreach (var customer in result)
			{
				ObjectDumper.Write($"Customer: {customer.CompanyName} TotalOrder: {customer.Orders.Sum(x => x.Total)}\n");
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 2")]
		[Description("List of suppliers for each customer")]

		public void Linq2()
		{
			var result = dataSource.Customers.Select(x => new
			{
				Customer = x,
				Suppliers = dataSource.Suppliers.Where(y => y.Country == x.Country && y.City == x.City).ToList()
			}).ToList();

			foreach (var client in result)
			{
				ObjectDumper.Write($"Client: {client.Customer.CompanyName}\n");
				foreach (var supplier in client.Suppliers)
				{
					ObjectDumper.Write($"Supplier: {supplier.SupplierName}\n");
				}
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 3")]
		[Description("List of clients, which had order with Total greater then condition value.")]
		public void Linq3()
		{
			var condition = 1;
			var result = dataSource.Customers.Where(x => x.Orders.Any(y => y.Total > condition)).ToList();

			foreach (var client in result)
			{
				ObjectDumper.Write($"Client: {client.CompanyName} \n");
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 4")]
		[Description("List of client with their first order date.")]
		public void Linq4()
		{
			var result = dataSource.Customers.Where(x => x.Orders.Length > 0).Select(x => new
			{
				Customer = x,
				StartOfCooperationYear = x.Orders.Select(y => y.OrderDate).Min().Year,
				StartOfCooperationMonth = x.Orders.Select(y => y.OrderDate).Min().Month
			}).ToList();

			foreach (var client in result)
			{
				ObjectDumper.Write($"Client: {client.Customer.CompanyName} FirstOrderYear: {client.StartOfCooperationYear} FirstOrderMonth: {client.StartOfCooperationMonth}\n");
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 5")]
		[Description("Task 4 sorted by year, month, total revenue and client name")]
		public void Linq5()
		{
			var result = dataSource.Customers
				.Where(x => x.Orders.Length > 0)
				.Select(x => new
				{
					Customer = x,
					StartOfCooperationYear = x.Orders.Select(y => y.OrderDate).Min().Year,
					StartOfCooperationMonth = x.Orders.Select(y => y.OrderDate).Min().Month,
					TotalRevenue = x.Orders.Sum(z => z.Total)
				}
			).OrderBy(x => x.StartOfCooperationYear)
			.ThenBy(x => x.StartOfCooperationMonth)
			.ThenByDescending(x => x.TotalRevenue)
			.ThenBy(x => x.Customer.CompanyName).ToList();

			foreach (var client in result)
			{
				ObjectDumper.Write($"Client: {client.Customer.CompanyName} FirstOrderYear: {client.StartOfCooperationYear} FirstOrderMonth: {client.StartOfCooperationMonth}\n");
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 6")]
		[Description("List of client with not digital post code.")]
		public void Linq6()
		{
			var result = dataSource.Customers.Where(x => string.IsNullOrEmpty(x.PostalCode) || !(x.Phone.StartsWith("(")) || string.IsNullOrEmpty(x.Region) || x.PostalCode.Any(y => y < '0' || y > '9') );

			foreach (var client in result)
			{
				ObjectDumper.Write($"Client: {client.CompanyName} PostCode: {client.PostalCode} Phone: {client.Phone} Region: {client.Region}\n");
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 7")]
		[Description("Products grouped by category, then by availability and then sorted by price.")]
		public void Linq7()
		{
			var result = dataSource.Products.GroupBy(x => x.Category)
				.Select(x => new
				{
					Category = x.Key,
					Products = x.GroupBy(y => y.UnitsInStock > 0)
					.Select(z => new
					{
						Available = z.Key,
						Product = z.OrderBy(a => a.UnitPrice)
					})
				}).ToList();

            foreach (var categorizedProducts in result)
            {
				ObjectDumper.Write($"Category: {categorizedProducts.Category}\n");
				foreach (var products in categorizedProducts.Products)
				{
					ObjectDumper.Write($"Has in stock: {products.Available}");
					foreach (var product in products.Product)
					{
						ObjectDumper.Write($"Product: {product.ProductName} Price: {product.UnitPrice}");
					}
				}
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 8")]
		[Description("Products grouped by 3 category (Cheap, average and expensive.")]
		public void Linq8()
		{
			var lowAverageBorder = 10;
			var highAverageBorder = 20;

			var result = dataSource.Products
				.GroupBy(x=>x.UnitPrice< lowAverageBorder ? "Cheap" : x.UnitPrice > highAverageBorder ? "Expensive" : "Average");

			foreach (var record in result)
			{
				ObjectDumper.Write($"Category: {record.Key}\n");
                foreach (var product in record)
                {
					ObjectDumper.Write($"Product: {product.ProductName}\n");
				}
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 9")]
		[Description("Average income of each year and average intisity for each city")]
		public void Linq9()
		{
			var result = dataSource.Customers.GroupBy(x => x.City)
				.Select(x => new
				{
					City = x.Key,
					AverageIncome = x.Average(y => y.Orders.Sum(z => z.Total)),
					AverageIntensity = x.Average(y => y.Orders.Length)
				}).ToList();

            foreach (var city in result)
            {
				ObjectDumper.Write($"City: {city.City}  -  AverageIncome: {city.AverageIncome}  AverageIntensity: {city.AverageIntensity}\n");
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 10")]
		[Description("Monthly, yearly and compound statistic for clients activity")]
		public void Linq10()
		{
			var result = dataSource.Customers
				.Select(x => new
				{
					Customer = x.CompanyName,
					MonthlyStatistic = x.Orders.GroupBy(y => y.OrderDate.Month)
					.Select(z => new
					{
						Month = z.Key,
						OrderCount = z.Count()
					}),
					YearlyStatistic = x.Orders.GroupBy(y => y.OrderDate.Year)
					.Select(z => new
					{
						Year = z.Key,
						OrderCount = z.Count()
					}),
					CompoundStatistic = x.Orders.GroupBy(y => y.OrderDate.Year)
					.Select(z => new
					{
						Year = z.Key,
						Month = z.GroupBy(a => a.OrderDate.Month)
						.Select(b => new
						{
							Month = b.Key,
							OrderCount = b.Count()
						})
					})
				});

            foreach (var statistic in result)
            {
				ObjectDumper.Write($"Customer: {statistic.Customer}\n");
                foreach (var record in statistic.CompoundStatistic)
                {
					ObjectDumper.Write($"Year: {record.Year}\n");
                    foreach (var month in record.Month)
                    {
						ObjectDumper.Write($"Month: {month.Month}  -  OrderCount: {month.OrderCount}\n");
					}
                }
			}
		}

	}
}
