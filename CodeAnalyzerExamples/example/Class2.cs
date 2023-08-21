using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using AutoMapper;
using AutoMapper;

namespace SummaryAddApp
{
    /// <summary>
    /// Class2
    /// </summary>
    public class Class2
    {
        /// <summary>
        /// Do
        /// </summary>
        public void Do(string[] args, string despoa)
        {
            Console.WriteLine("Hello, world!");
        }
    }

    public class ClassControler
    {
        /// <summary>
        /// Method adds new document type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ResponseType(typeof(M2MApiModels.Response.DocumentType))]
        [Route("add")]
        public HttpResponseMessage Add(M2MApiModels.Request.AddDocumetType model)
        {


            dbDocumentType.Id = Guid.NewGuid();

            if (UnitOfWork.DocumentTypeRepository.CodeExists(model.Code))
            {
                Logger.Error("Error adding document type: Document Type Code Must Be Unique");
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "Document Type Code Must Be Unique");
            }

            UnitOfWork.DocumentTypeRepository.Add(dbDocumentType);
            UnitOfWork.SaveChanges();

        }

        /// <summary>
        ///  Method updates existing document type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ResponseType(typeof(M2MApiModels.Response.DocumentType))]
        [Route("update")]
        public HttpResponseMessage Update(M2MApiModels.Request.UpdateDocumentType model)
        {
            DocumentType dbDocumentType = null;

            if (model.Id != Guid.Empty)
            {
                dbDocumentType = UnitOfWork.DocumentTypeRepository.Get(model.Id);
            }
            else if (!String.IsNullOrEmpty(model.Code))
            {
                dbDocumentType = UnitOfWork.DocumentTypeRepository.GetByCode(model.Code);
            }

            if (dbDocumentType != null)
            {
                dbDocumentType.Name = model.Name;

                UnitOfWork.SaveChanges();
            }

            Logger.Error("Error updating document type: Document type not found");

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }


    [Route("{tenantDomain}/api/create-form-field")]
    public class CreateFormFieldController : WebApi.BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IMapper _mapper;

        public CreateFormFieldController(IUnitOfWork uow, ILog log, IMapper mapper, IMapper mapper) : base(uow, log)
        {
            _mapper = mapper;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("non-responsive")]
        public IActionResult GetAll()
        {
            return Ok(UnitOfWork.CreateFormFieldRepository.GetAll().Select(x => Mapper.Map<CreateFormFieldModel>(x)));
        }

        [HttpGet]
        [Route("responsive")]
        public IActionResult GetAllResponsive()
        {
            //write route 
            return Ok(UnitOfWork.CreateFormFieldRepository.GetAll().Where(x => x.IsResponsive).Select(x => Mapper.Map<CreateFormFieldModel>(x)));
        }

        //[AuthorizeUser(PermissionEnum.WidgetFieldMaterial)]
        [HttpPost]
        public IActionResult AddCreateFormFields(List<CreateFormFieldModel> model)
        {
            List<CreateFormField> createFormFields = UnitOfWork.CreateFormFieldRepository.GetAll().ToList();

            foreach (var createFormField in createFormFields)
            {

                try
                {
                    UnitOfWork.CreateFormFieldRepository.Delete(createFormField);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            foreach (var field in model)
            {
                CreateFormField formField = new CreateFormField()
                {
                    Name = field.Name,
                    Key = field.Key,
                    IsResponsive = field.IsResponsive,
                    Order = field.Order,
                    IsDate = field.IsDate
                };

                try
                {
                    UnitOfWork.CreateFormFieldRepository.Add(formField);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }


            try
            {
                UnitOfWork.SaveChanges();
                return Ok(model);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
