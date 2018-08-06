using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moments.Model.EF;
using Moments.Models;

namespace Moments.RepositoryDAL
{
    public class MomentDAL
    {
        MomentsEntities _db = new MomentsEntities();
        #region Stories 
        public List<Sp_GetStories_Result> GetStories(int? UserId ,int? CategoryId, int? Status, bool? IsFeatures )
        {
            try
            {
                List<Sp_GetStories_Result> GetAllStoreis = new List<Sp_GetStories_Result>();
                GetAllStoreis = _db.Sp_GetStories(IsFeatures, Status, CategoryId, UserId,null).ToList();
                return GetAllStoreis;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Node> GetAllNodes()
        {
            using (var dbContext = new MomentsEntities())
            {
                var allNodes = dbContext.Nodes.ToList();
                return allNodes;
            }
        }

        public List<Story> GetAllStories()
        {
            using (var dbContext = new MomentsEntities())
            {
                var allNodes = dbContext.Stories.ToList();
                return allNodes;
            }
        }


        public UserProfile GetUserByProfileId(string userId)
        {
            using (var dbContext = new MomentsEntities())
            {
                var user = dbContext.UserProfiles.FirstOrDefault(o => o.Id.ToString() == userId);
                return user;
            }
        }


        public List<StoriesNode> GetStoryNodes(int? StoryIdDynamic)
        {
            try
            {
                List<StoriesNode> StoriesNodeList = new List<StoriesNode>();
                StoriesNodeList = _db.StoriesNodes.Where(x => x.StoryId == StoryIdDynamic).ToList();
                return StoriesNodeList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Sp_GetStories_Result GetStoryData(int? StoryId)
        {
            try
            {
                Sp_GetStories_Result GetStory = new Sp_GetStories_Result();
                GetStory = _db.Sp_GetStories(null, null, null, null, StoryId).FirstOrDefault();
                return GetStory;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public int SaveStoryNodes(List<StoriesNode> storiessodelist)
        {
            try
            {
                //removing all data first
                List<StoriesNode> StoriesNodeForRemoving = new List<StoriesNode>();
                int storyid = Convert.ToInt32(storiessodelist[0].StoryId);
               StoriesNodeForRemoving = _db.StoriesNodes.Where(x => x.StoryId == storyid).ToList();
                _db.StoriesNodes.RemoveRange(StoriesNodeForRemoving);

                //adding new data
                int status = 0;
                _db.StoriesNodes.AddRange(storiessodelist);
                _db.SaveChanges();
                status = 1;
                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public int AddStory(Story Story)
        {
            try
            {
                int status = 0;
                _db.Stories.Add(Story);
                _db.SaveChanges();
                status = 1;
                return Story.Id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion


        public ReportingModel GetReporting(GetNetworkRequestModel model)
        {
            var nodes = new List<NodeModel>();
            var rels = new List<Relation>();
            var ownerName = "";
            var storyName = "";
            var img = "";

            using (var dbContext = new MomentsEntities())
            {
                var story = dbContext.Stories.FirstOrDefault(sto => sto.Id.ToString() == model.StoryId);
                if(story != null)
                {
                    storyName = story.Name;
                    img = story.Path;
                    var bunda = dbContext.UserProfiles.FirstOrDefault(user => user.Id == story.CreatedBy);
                    if (bunda != null)
                    {
                        ownerName = bunda.FirstName + " " + bunda.LastName;
                    }
                }
                var allNodes = dbContext.Nodes.Where(node => node.StoryId.ToString()==model.StoryId).ToList();
                if(allNodes!=null && allNodes.Any())
                {
                    foreach (var item in allNodes)
                    {
                        nodes.Add(NodeMapper(item));
                        if (item.ParentId != 0)
                        {
                            rels.Add(new Relation
                            {
                                from= item.ParentId.ToString(),
                                to= item.Id.ToString(),
                                color= "#6c757d"
                            });
                        }
                    }
                }
            }
            return new ReportingModel { Nodes = nodes, Relation = rels , Owner= ownerName, Image= img, StoryName= storyName };
        }

        public List<Node> GetStoryNodes(string storyId)
        {
            using (var dbContext = new MomentsEntities())
            {
                var allNodes = dbContext.Nodes.Where(node => node.StoryId.ToString() == storyId).ToList();
                return allNodes;
            }
        }

        public int GetStoryBranches(string storyId)
        {
            int count = 0;
            var allNodes = GetStoryNodes(storyId);
            if (allNodes != null)
            {
                foreach(var item in allNodes)
                {
                    var result = allNodes.Count(obj => obj.ParentId == item.Id);
                    if (result == 0)
                    {
                        count++;
                    }
                }
            }
            return count;
        }



        private NodeModel NodeMapper(Node source)
        {
            var contents = "Text Node";
            var shape = "";
            var borderGroup = "";
            if (source.Type == "Text")
            {
                if (source.Contents.Length > 10)
                {
                    contents = source.Contents.Substring(0, 10) + "...";
                }
                shape = "box";
                borderGroup = "highFriendliness";
            }
            else if (source.Type == "Image")
            {
                contents = "Image Node";
                shape = "image";
                borderGroup = "extremeFriendliness";
            }
            else if (source.Type == "Video")
            {
                if (source.Contents.Length > 20)
                {
                    contents = source.Contents.Substring(0, 20) + "...";
                }
                shape = "box";
                borderGroup = "unknownFriendliness";
            }
            else if (source.Type == "Audio")
            {
                if (source.Contents.Length > 20)
                {
                    contents = source.Contents.Substring(0, 20) + "...";
                }
                shape = "box";
                borderGroup = "notAssignedFriendliness";
            }

            var obj = new NodeModel
            {
                id=source.Id.ToString(),
                label= contents,
                type= source.Type,
                shape= shape,
                size=40,
                group= borderGroup,
                FriendlinessWeightColor= "border: 1px solid green;",
                borderWidth=1,
                Contents= source.Contents,
                parentId= source.ParentId.ToString(),
                storyId= source.StoryId.ToString()
            };
            return obj;
        }

        public long AddNewNode(Node source)
        {
            using (var dbContext = new MomentsEntities())
            {
               dbContext.Nodes.Add(source);
               dbContext.SaveChanges();
                return source.Id;
            }
        }


        public NodeModel GetNodeById(string nodeId)
        {
            using (var dbContext = new MomentsEntities())
            {
                var obj=  dbContext.Nodes.FirstOrDefault(o => o.Id.ToString() == nodeId);
                if (obj != null)
                {
                    return NodeMapper(obj);
                }
            }
            return null;
        }


        public Story GetStoryById(string storyId)
        {
            using (var dbContext = new MomentsEntities())
            {
                var obj = dbContext.Stories.FirstOrDefault(o => o.Id.ToString() == storyId);
                return obj;
            }
        }
    }
}