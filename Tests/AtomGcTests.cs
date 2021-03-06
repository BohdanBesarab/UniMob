using NUnit.Framework;

namespace UniMob.Tests
{
    public class AtomGcTests
    {
        [Test]
        public void NoSubscribed()
        {
            var source = Atom.Value(1);
            var middle = Atom.Computed(() => source.Value + 1);
            var target = Atom.Computed(() => middle.Value + source.Value);

            Assert.AreEqual(3, target.Value);

            Assert.AreEqual(0, source.SubscribersCount());
            Assert.AreEqual(0, middle.SubscribersCount());
            Assert.AreEqual(0, target.SubscribersCount());
        }

        [Test]
        public void AutoUnsubscribed_Reaction()
        {
            var source = Atom.Value(1);
            var middle = Atom.Computed(() => source.Value + 1);
            var target = Atom.Computed(() => middle.Value + source.Value);

            var run = Atom.Reaction(() => target.Get());

            Assert.AreEqual(2, source.SubscribersCount());
            Assert.AreEqual(1, middle.SubscribersCount());
            Assert.AreEqual(1, target.SubscribersCount());

            run.Deactivate();
            AtomScheduler.Sync();

            Assert.AreEqual(0, source.SubscribersCount());
            Assert.AreEqual(0, middle.SubscribersCount());
            Assert.AreEqual(0, target.SubscribersCount());
        }

        [Test]
        public void KeepAliveComputed()
        {
            var source = Atom.Value(1);
            var middle = Atom.Computed(() => source.Value + 1, keepAlive: true);
            var target = Atom.Computed(() => middle.Value + source.Value);

            var run = Atom.Reaction(() => target.Get());

            Assert.AreEqual(2, source.SubscribersCount());
            Assert.AreEqual(1, middle.SubscribersCount());
            Assert.AreEqual(1, target.SubscribersCount());

            run.Deactivate();
            AtomScheduler.Sync();

            Assert.AreEqual(1, source.SubscribersCount());
            Assert.AreEqual(0, middle.SubscribersCount());
            Assert.AreEqual(0, target.SubscribersCount());

            middle.Deactivate();
            AtomScheduler.Sync();

            Assert.AreEqual(0, source.SubscribersCount());
        }

        [Test]
        public void AutoUnsubscribed_MultiReaction()
        {
            var source = Atom.Value(1);
            var middle = Atom.Computed(() => source.Value + 1);
            var target1 = Atom.Computed(() => middle.Value + 1);
            var target2 = Atom.Computed(() => middle.Value + 1);

            var run1 = Atom.Reaction(() => target1.Get());
            var run2 = Atom.Reaction(() => target2.Get());

            Assert.AreEqual(2, middle.SubscribersCount());

            run1.Deactivate();
            AtomScheduler.Sync();

            Assert.AreEqual(1, middle.SubscribersCount());

            run2.Deactivate();
            AtomScheduler.Sync();

            Assert.AreEqual(0, middle.SubscribersCount());
        }
    }
}