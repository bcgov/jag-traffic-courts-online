package ca.bc.gov.open.jag.tco.ocr.metrics.repository;

import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.TypedQuery;

import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.ocr.metrics.model.FieldComparison;
import ca.bc.gov.open.jag.tco.ocr.metrics.model.Source;

@Repository
public class QueryRepository {

    @PersistenceContext
    private EntityManager entityManager;

	public List<FieldComparison> customQuery(Source source1, Source source2) {
		StringBuffer sb = new StringBuffer();
		sb.append("select new ca.bc.gov.open.jag.tco.ocr.metrics.model.FieldComparison(d1.fileName, f1.fieldName, f1.content, f2.content, f2.confidence) ");
		sb.append("from Document d1 join d1.fields f1, Document d2 join d2.fields f2 ");
		sb.append("where d1.source = ?1 ");
		sb.append("and d2.source = ?2 ");
		sb.append("and d1.fileName = d2.fileName ");
		sb.append("and f1.fieldName = f2.fieldName ");
		sb.append("and (");
		sb.append("  (f1.content is null and f2.content is not null) or ");
		sb.append("  (f1.content is not null and f2.content is null) or ");
		sb.append("  (lower(f1.content) != lower(f2.content)) ");
		sb.append(" )");
		sb.append("and (length(f1.content) > 0 or length(f2.content) > 0) ");
		TypedQuery<FieldComparison> query = entityManager.createQuery(sb.toString(), FieldComparison.class);
		query.setParameter(1, source1);
		query.setParameter(2, source2);
		List<FieldComparison> list = query.getResultList();
		return list;
	}

	public Long getTotalFields() {
		StringBuffer sb = new StringBuffer();
		sb.append("select count(*) ");
		sb.append("from Document d1 join d1.fields f1 ");
		sb.append("where d1.source = ?1 ");
		TypedQuery<Long> query = entityManager.createQuery(sb.toString(), Long.class);
		query.setParameter(1, Source.HUMAN);
		List<Long> result = query.getResultList();
		return result.get(0);
	}
}
