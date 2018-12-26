using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<ProductsController> _logger;
        private readonly IMapper _mapper;

        public OrdersController(IDutchRepository repository, ILogger<ProductsController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(bool includeItems = true)
        {
            try
            {
                var results = _repository.GetAllOrders(includeItems);
                return Ok
                (
                    _mapper.Map<IEnumerable<Order>,
                        IEnumerable<OrderViewModel>>(results)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all orders: {ex}");
                return BadRequest("Failed to get all orders");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = _repository.GetOrderById(id);

                if (order != null)
                {
                    return Ok(_mapper.Map<Order, OrderViewModel>(order));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get an order: {ex}");
                return BadRequest("Failed to get an order");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
//                    var newOrder = new Order()
//                    {
//                        OrderDate = model.OrderDate,
//                        OrderNumber = model.OrderNumber,
//                        Id = model.OrderId
//                    };

                    var newOrder = _mapper.Map<OrderViewModel, Order>(model);

                    if (newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }

                    _repository.AddEntity(newOrder);

                    if (_repository.SaveAll())
                    {
//                        var vm = new OrderViewModel()
//                        {
//                            OrderId = newOrder.Id,
//                            OrderDate = newOrder.OrderDate,
//                            OrderNumber = newOrder.OrderNumber
//                        };

//                        return Created($"/api/orders/{vm.OrderId}", vm);
                        return Created
                        (
                            $"/api/orders/{newOrder.Id}",
                            _mapper.Map<Order, OrderViewModel>(newOrder)
                        );
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save a new order: {ex}");
                return BadRequest("Failed to save a new order");
            }
        }
    }
}