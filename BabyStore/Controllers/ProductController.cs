using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BabyStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BabyStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IEntityRepository<Product> repository;
        private readonly IEntityRepository<Category> catRepo;
        private readonly IEntityRepository<ProductImage> imgRepo;
        private readonly IEntityRepository<ProductImageMapping> mapRepo;

        public ProductController(IEntityRepository<Product> repository,
            IEntityRepository<Category> catRepo,
            IEntityRepository<ProductImage> imgRepo
            ,IEntityRepository<ProductImageMapping> mapRepo)
        {
            this.repository = repository;
            this.catRepo = catRepo;
            this.imgRepo = imgRepo;
            this.mapRepo = mapRepo;
        }
        public IActionResult Index(int categoryId,int page=1)
        {
          
            ProductsIndexVM model =new ProductsIndexVM {
               CategoryId=categoryId,
               Categories=catRepo.GetAll().ToList<Category>()
              
            };
            if (categoryId == default(int))
            {
                model.Products = repository
                   .Paginate(page,Constants.PageSize, p => p.ID, null, p => p.Category);
            }
            else
            {
                model.Products = repository
                  .Paginate(page, Constants.PageSize, p => p.ID, 
                  p=>p.CategoryID==categoryId, p => p.Category);
            }
            return View(model);
        }


       


        public IActionResult Create()
        {
            CreateProductVM vModel = new CreateProductVM();
           // vModel.CategoryList = new SelectList(imgRepo.GetAll(), "ID", "FileName");

            vModel.CategoryList = new SelectList(catRepo.GetAll(), "ID", "Name");

            vModel.ImageLists = new List<SelectList>();
            for (int i = 0; i < Constants.NumberOfProductImages; i++)
            {
                vModel.ImageLists.Add(new SelectList(imgRepo.GetAll(), "ID", "FileName"));

            }
            return View(vModel);
        }
        [HttpPost]
        public IActionResult Create(CreateProductVM newProduct)
        {
            Product product = new Product();
            product.Name = newProduct.Name;
            product.Description = newProduct.Description;
            product.Price = newProduct.Price;
            product.CategoryID = newProduct.CategoryID;
            product.ProductImageMappings = new List<ProductImageMapping>();
            string[] productImages = newProduct.ProductImages
                .Where(pi => !string.IsNullOrEmpty(pi)).ToArray();
            for (int i = 0; i < productImages.Length; i++)
            {
                product.ProductImageMappings.Add(new ProductImageMapping
                {
                    ProductImage = imgRepo.GetSingle(int.Parse(productImages[i])),
                  //  ProductImageID=int.Parse(productImages[i]),
                    ImageNumber = i
                }) ;
            }
            if (ModelState.IsValid)
            {
                repository.Add(product);
                repository.Save();
                return RedirectToAction(nameof(Index));
            }
            newProduct.CategoryList = new SelectList(catRepo.GetAll(), "ID", "Name", product.CategoryID);
            newProduct.ImageLists = new List<SelectList>();
            for (int i = 0; i < Constants.NumberOfProductImages; i++)
            {
                newProduct.ImageLists.Add(new SelectList(imgRepo.GetAll(), "ID", "FileName",
                newProduct.ProductImages[i]));
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            Product product = repository.GetSingle(id);
            product.ProductImageMappings = mapRepo
                .FindBy(m => m.ProductID == product.ID).ToList();
            CreateProductVM model = new CreateProductVM();
            model.CategoryList = new SelectList(catRepo.GetAll(),
                "ID", "Name",product.CategoryID);

            model.ImageLists = new List<SelectList>();
            foreach (var imageMapping in product.ProductImageMappings
                .OrderBy(i => i.ImageNumber))
            {
                model.ImageLists.Add(new SelectList(imgRepo.GetAll(),
                    "ID", "FileName", imageMapping.ProductImageID));
            }
            for (int i = product.ProductImageMappings.Count();
                i < Constants.NumberOfProductImages; i++)
            {
                model.ImageLists.Add(new SelectList(imgRepo.GetAll(), "ID", "FileName"));
            }
            model.ID = product.ID;
            model.Name = product.Name;
            model.Description = product.Description;
            model.Price = product.Price;
            model.CategoryID =(int)product.CategoryID;
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(CreateProductVM model)
        {
            Product productToUpdate = repository
                .AllIncluding(p => p.ProductImageMappings,x=>x.Category).First(p => p.ID == model.ID);

            productToUpdate.CategoryID = model.CategoryID;
            productToUpdate.Name = model.Name;
            productToUpdate.Description = model.Description;
            productToUpdate.Price = model.Price;
           
            if(productToUpdate.ProductImageMappings==null)
            {
                productToUpdate.ProductImageMappings =
                    new List<ProductImageMapping>();
            }
            string[] productImages = model.ProductImages
                .Where(pi => !string.IsNullOrEmpty(pi)).ToArray();
            for (int i = 0; i < productImages.Length; i++)
            {
                var imageMappingToEdit = productToUpdate.ProductImageMappings
                    .Where(pim => pim.ImageNumber == i).FirstOrDefault();
                var image = imgRepo.GetSingle(int.Parse(productImages[i]));
                if (imageMappingToEdit == null)
                {
                    productToUpdate.ProductImageMappings.Add(
                        new ProductImageMapping {
                         ProductImage=image,
                          ProductImageID=image.ID,
                           ImageNumber=i
                        });
                }
                else
                {
                    if(imageMappingToEdit.ProductImageID!=
                        int.Parse(productImages[i]))
                    {
                        imageMappingToEdit.ProductImage = image;
                    }
                }
            }

            for (int i = productImages.Length;
                i < Constants.NumberOfProductImages; i++)
            {
                var imageMappingToEdit = productToUpdate
                    .ProductImageMappings.Where(pim =>pim.ImageNumber == i)
                    .FirstOrDefault();
                if (imageMappingToEdit != null)
                {
                    mapRepo.Delete(imageMappingToEdit);
                    
                }
            }
           
            
                repository.Edit(productToUpdate);
                repository.Save();
                return RedirectToAction(nameof(Index));

            
           
        }

    }
}