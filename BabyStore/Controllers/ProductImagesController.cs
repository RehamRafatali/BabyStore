using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BabyStore.Infrastrcture;
using BabyStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace BabyStore.Controllers
{
    public class ProductImagesController : Controller
    {
        private readonly IEntityRepository<ProductImage> repository;
        private readonly FileValidation validation;

        public ProductImagesController(IEntityRepository<ProductImage> repository
        ,FileValidation validation )
        {
            this.repository = repository;
           this.validation = validation;
        }
        public IActionResult Index()
        {

            return View(repository.GetAll());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(IFormFile[] files)
        {
            bool AllValid = true;
            string inValid = "";

            if (files[0] != null)
            {
                if (files.Length <= 10)
                {
                    foreach (IFormFile file in files)
                    {
                        if (!validation.ValidateFile(file))
                        {
                            AllValid = false;
                            inValid += ", " + file.FileName;
                        }
                    }
                    if (AllValid)
                    {
                        foreach (IFormFile file in files)
                        {
                            try
                            {
                                validation.SaveFileToDisk(file);
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("FileName", 
                                    "Sorry an error occurred saving the files to disk, please try again");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("FileName",
                      "All files must be gif, png, jpeg or jpg and less  than 2MB in size.The following files" + inValid + " are not valid");
                    }
                     

                }
                else
                {
                    ModelState.AddModelError("FileName", 
                        "Please only upload up to ten files at a time");
}
            }
            else
            {
                ModelState.AddModelError("FileName", "Please choose a file");
            }
            if (ModelState.IsValid)
            {
                bool duplicates = false;
                bool otherDbError = false;
                string duplicateFiles = "";
                foreach (IFormFile file in files)
                {
                    ProductImage productImage = new ProductImage { FileName = file.FileName };
                    try
                    {
                        repository.Add(productImage);
                        repository.Save();
                    }
                    catch (Exception e)
                    {                      
                            duplicateFiles += ", " + file.FileName;
                            duplicates = true;
                        repository.Detache(productImage);
                    }
                   
                }
                if (duplicates)
                {
                    ModelState.AddModelError("FileName", "All files uploaded except the files" +
                    duplicateFiles + ", which already exist in the system." + " Please delete them and try again if you wish to re - add them");
                    return View();
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}