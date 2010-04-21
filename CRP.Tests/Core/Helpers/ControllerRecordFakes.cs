using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Testing;

namespace CRP.Tests.Core.Helpers
{
    /// <summary>
    /// Static methods to fake collections of records for controller tests.
    /// </summary>
    public static class ControllerRecordFakes
    {
        /// <summary>
        /// Fakes the QuestionSets.
        /// Author: Sylvestre, Jason
        /// Create: 2010/04/01
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="count">The number of QuestionSets to add.</param>
        public static void FakeQuestionSets(List<QuestionSet> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.QuestionSet(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the users.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeUsers(List<User> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.User(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the question types.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeQuestionTypes(List<QuestionType> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.QuestionType(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the validators.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeValidators(List<Validator> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Validator(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the units.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeUnits(List<Unit> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Unit(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the items.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeItems(List<Item> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Item(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the item question sets.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeItemQuestionSets(List<ItemQuestionSet> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.ItemQuestionSet(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the editors.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeEditors(List<Editor> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Editor(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the schools.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeSchools(List<School> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.School(i + 1 + offSet));
                entity[i + offSet].SetIdTo((i + 1 + offSet).ToString());
            }
        }

        /// <summary>
        /// Fakes the questions.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeQuestions(List<Question> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Question(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the item types.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeItemTypes(List<ItemType> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.ItemType(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
    }
}
