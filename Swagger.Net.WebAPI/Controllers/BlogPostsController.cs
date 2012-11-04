﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swagger.Net.WebApi.Models;

namespace Swagger.Net.WebApi.Controllers
{
    public class BlogPostsController : ApiController
    {
        /// <summary>
        /// BlogPostsController.Get Summary
        /// </summary>
        /// <remarks>Here are some operation remarks</remarks>
        /// <returns>
        /// <see cref="BlogPost"/>a collection of blog posts
        /// </returns>
        public IEnumerable<BlogPost> Get()
        {
            return new List<BlogPost> { new BlogPost(), new BlogPost() };
        }


        /// <summary>
        /// BlogPostsController.Get(id) Summary
        /// </summary>
        /// <remarks>Here are some remarks about the Get by id</remarks>
        /// <param name="id">this is id of post you are looking for</param>
        /// <returns>
        /// <see cref="BlogPost"/>
        /// some comments about returned blogpost
        /// </returns>
        
        public BlogPost Get(int id)
        {
            return new BlogPost();
        }

        /// <summary>
        /// Post summary
        /// </summary>
        /// <remarks>This shows up as notes</remarks>
        /// <param name="blogPost"><see cref="BlogPost"/>        
        /// This is a description of blogpost param.
        /// </param>
        /// <returns><see cref="HttpResponseMessage"/>Status code 200 for ok</returns>
        public HttpResponseMessage Post(BlogPost value)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// BlogPostsController.Put
        /// </summary>
        /// <param name="id"><see cref="Int32"/></param>
        /// <param name="value"><see cref="BlogPost"/>The post to put to database</param>
        public void Put(int id, BlogPost value)
        {
        }

        /// <summary>
        /// Deletes a blogpost
        /// </summary>
        /// <remarks>
        /// This function deletes a blog post if user has permission
        /// </remarks>
        /// <param name="id"><see cref="Int32"/>The integer id of the post</param>
        public void Delete(int id)
        {
        }
    }
}
